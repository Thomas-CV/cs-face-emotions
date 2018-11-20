using CognitiveServicesFace.Models;
using CognitiveServicesFace.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CognitiveServicesFace.Pages
{
	public class IndexModel : PageModel
	{
		private static class CacheKeys
		{
			public const string Images = "img";
		}
		private const string RootFolderName = "wwwroot\\";
		private const string ImagesFolderName = RootFolderName + "images";

		private IConfiguration Configuration { get; }
		private IMemoryCache MemoryCache { get; }
		public string MessageText { get; private set; } = "null";
		public string ImageFilename { get; private set; } = "ImagePlaceholder.png";
		public ImageInfo ImageInformation { get; set; } = new ImageInfo() { BoundingBoxes = "", Description = "" };
		public int ImageIndex { get; private set; } = 1;

		public IndexModel(IConfiguration configuration, IMemoryCache cache)
		{
			Configuration = configuration;
			MemoryCache = cache;
		}


		public async Task OnGet(int? index)
		{
			if (index.HasValue == false || index.Value < 1) return;

			string filename = null;
			var images = GetListOfImages();

			if (images != null && images.Length > 0)
			{
				if (index < images.Length) ImageIndex = index.Value + 1;
				else ImageIndex = 1;

				if (--index >= images.Length) index = 0;
				filename = images[index.Value];
				ImageFilename = filename.Replace(RootFolderName, String.Empty);
			}
			if (ImageIndex != 0 && String.IsNullOrEmpty(filename) == false)
			{
				string subscriptionKey = Configuration.GetValue<string>("SubscriptionKey");
				string serviceEndpoint = Configuration.GetValue<string>("ServiceEndpoint");
				MessageText = await Face.AnalyzeEmotions(filename, subscriptionKey, serviceEndpoint);
			}

			ImageInformation.BoundingBoxes = CollectBoundingBoxes(MessageText);
		}

		private string[] GetListOfImages()
		{
			string[] images = MemoryCache.Get<string[]>(CacheKeys.Images);
			if (images == null)
			{
				images = Directory.GetFiles(ImagesFolderName);
				MemoryCache.Set(CacheKeys.Images, images);
			}
			return images;
		}

		private string CollectBoundingBoxes(string text)
		{
			if (String.IsNullOrEmpty(text)) return String.Empty;

			try
			{
				var result = String.Empty;
				var data = Newtonsoft.Json.Linq.JArray.Parse(text);

				foreach (var item in data)
				{
					var rectangle = item["faceRectangle"];
					result += rectangle["top"] + "," + rectangle["left"] + "," + rectangle["width"] + "," + rectangle["height"] + ",";
				}
				if (String.IsNullOrEmpty(result) == false) return result.TrimEnd(',');
			}
			catch
			{
				// TODO: handle exception here
			}
			return String.Empty;
		}
	}
}
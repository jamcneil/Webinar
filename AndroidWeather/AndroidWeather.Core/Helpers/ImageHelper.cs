using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Weather.Helpers
{
	public static class ImageHelper
	{
		
		public static async Task<byte[]> GetImageBytesAsync (string url)
		{
			byte [] imageBytes;
			using (var httpClient = new HttpClient ()) {
						imageBytes = await httpClient.GetByteArrayAsync (url);}
			return imageBytes;
		}
	}
}

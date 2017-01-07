using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System;
using Weather.ViewModels;
using Weather.Models;
using Weather.Helpers;
using Android.Views.Animations;
using System.Threading.Tasks;

namespace AndroidWeather
{
	[Activity (Label = "Weather", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		FrameLayout progressFrame;
		ImageView imageViewFirst, imageViewSecond, imageViewThird, imageViewFourth;
		TextView firstHour, secondHour, thirdHour, fourthHour;
		TextView firstHourWeather, secondHourWeather, thirdHourWeather, fourthHourWeather;
		TextView tempText;
		TextView weatherText;
		Bitmap imageBitmap1, imageBitmap2, imageBitmap3, imageBitmap4 = null;
		WeatherViewModel wvm;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Title = "Weather";
			SetContentView (Resource.Layout.Main);

			Button getWeather = FindViewById<Button> (Resource.Id.getWeatherButton);
			getWeather.Click += OnGetWeatherClicked;

			progressFrame = FindViewById<FrameLayout> (Resource.Id.progressBarHolder);
			imageViewFirst = FindViewById<ImageView> (Resource.Id.imageViewFirst);
			imageViewSecond = FindViewById<ImageView> (Resource.Id.imageViewSecond);
			imageViewThird = FindViewById<ImageView> (Resource.Id.imageViewThird);
			imageViewFourth = FindViewById<ImageView> (Resource.Id.imageViewFourth);
			firstHour = FindViewById<TextView> (Resource.Id.firstHour);
			secondHour = FindViewById<TextView> (Resource.Id.secondHour);
			thirdHour = FindViewById<TextView> (Resource.Id.thirdHour);
			fourthHour = FindViewById<TextView> (Resource.Id.fourthHour);
			firstHourWeather = FindViewById<TextView> (Resource.Id.firstHourWeather);
			secondHourWeather = FindViewById<TextView> (Resource.Id.secondHourWeather);
			thirdHourWeather = FindViewById<TextView> (Resource.Id.thirdHourWeather);
			fourthHourWeather = FindViewById<TextView> (Resource.Id.fourthHourWeather);
			tempText = FindViewById<TextView> (Resource.Id.tempText);
			weatherText = FindViewById<TextView> (Resource.Id.weatherText);
		}

		async void OnGetWeatherClicked (object sender, EventArgs e)
		{
			Settings.IsImperial = FindViewById<Switch> (Resource.Id.switchImperial).Checked;
			Settings.UseCity = !FindViewById<Switch> (Resource.Id.switchGPS).Checked;
			Settings.City = FindViewById<EditText> (Resource.Id.cityText).Text;

			wvm = new WeatherViewModel ();

			AlphaAnimation inAnimation = new AlphaAnimation (0f, 1f);
			inAnimation.Duration = 200;
			progressFrame.Animation = inAnimation;
			progressFrame.Visibility = Android.Views.ViewStates.Visible;

			await wvm.ExecuteGetWeatherCommand ();

			if (wvm.IsDataAvailable) {

				await GetBitmapsAsync ();
				//first window
				imageViewFirst.SetImageBitmap (imageBitmap1);
				firstHour.Text = wvm.Forecast.Items [0].DisplayTime;
				firstHourWeather.Text = wvm.Forecast.Items [0].DisplayTemp;
				//second window
				imageViewSecond.SetImageBitmap (imageBitmap2);
				secondHour.Text = wvm.Forecast.Items [1].DisplayTime;
				secondHourWeather.Text = wvm.Forecast.Items [1].DisplayTemp;
				//third window
				imageViewThird.SetImageBitmap (imageBitmap3);
				thirdHour.Text = wvm.Forecast.Items [2].DisplayTime;
				thirdHourWeather.Text = wvm.Forecast.Items [2].DisplayTemp;
				//fourth window
				imageViewFourth.SetImageBitmap (imageBitmap4);
				fourthHour.Text = wvm.Forecast.Items [3].DisplayTime;
				fourthHourWeather.Text = wvm.Forecast.Items [3].DisplayTemp;

				tempText.Text = wvm.Temp;
				weatherText.Text = wvm.Condition;

				LinearLayout hiddenLayout = FindViewById<LinearLayout> (Resource.Id.linearLayoutHidden);
				AlphaAnimation outAnimation = new AlphaAnimation (0f, 1f);
				outAnimation.Duration = 100;
				progressFrame.Animation = outAnimation;
				progressFrame.Visibility = Android.Views.ViewStates.Gone;

				LinearLayout mainLayout = FindViewById<LinearLayout> (Resource.Id.mainLayout);
				if (wvm.Condition.ToLower ().Contains ("sun") || wvm.Condition.ToLower ().Contains ("clear"))
					mainLayout.SetBackgroundResource (Resource.Drawable.sun);
				else if (wvm.Condition.ToLower ().Contains ("snow"))
					mainLayout.SetBackgroundResource (Resource.Drawable.snow);
				else if (wvm.Condition.ToLower ().Contains ("rain"))
					mainLayout.SetBackgroundResource (Resource.Drawable.rain);
				else mainLayout.SetBackgroundResource (Resource.Drawable.cloud);

				hiddenLayout.Visibility = Android.Views.ViewStates.Visible;
			}
		}
		public async Task GetBitmapsAsync ()
		{
			var imageBytes = await ImageHelper.GetImageBytesAsync (wvm.Forecast.Items [0].DisplayIcon).ConfigureAwait (false);
			if (imageBytes != null && imageBytes.Length > 0)
				imageBitmap1 = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
			imageBytes = await ImageHelper.GetImageBytesAsync (wvm.Forecast.Items [1].DisplayIcon).ConfigureAwait (false);
			if (imageBytes != null && imageBytes.Length > 0)
				imageBitmap2 = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
			imageBytes = await ImageHelper.GetImageBytesAsync (wvm.Forecast.Items [2].DisplayIcon).ConfigureAwait (false);
			if (imageBytes != null && imageBytes.Length > 0)
				imageBitmap3 = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
			imageBytes = await ImageHelper.GetImageBytesAsync (wvm.Forecast.Items [3].DisplayIcon).ConfigureAwait (false);
			if (imageBytes != null && imageBytes.Length > 0)
				imageBitmap4 = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
		}

	}

}




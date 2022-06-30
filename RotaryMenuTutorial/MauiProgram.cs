using Microsoft.AspNetCore.Components.WebView.Maui;
using RotaryMenuTutorial.Pages;
using RotaryMenuTutorial.ViewModel;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace RotaryMenuTutorial;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		
        builder.UseSkiaSharp(true);

		builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<EditMenuViewModel>();
        builder.Services.AddTransient<EditMenuPage>();



        return builder.Build();
	}
}

using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Entry = Microsoft.Maui.Controls.Entry;

namespace StatusBarRepro;

public class MainPage : ContentPage
{
	public MainPage()
	{
		Title = "StatusBarBehavior repro";
		BackgroundColor = Colors.White;

		Behaviors.Add(new StatusBarBehavior
		{
			StatusBarColor = Colors.White,
			StatusBarStyle = StatusBarStyle.DarkContent
		});

		Content = new VerticalStackLayout
		{
			Padding = new Thickness(16, 32),
			Spacing = 16,
			Children =
			{
				new Label
				{
					Text = "Bug 1: tap the Entry (keyboard), dismiss it, push the next page (it applies the StatusBarBehavior again), then open the full screen modal. The white status bar stays on top of the modal.\n\n" +
						"Bug 2: open the native sheet (UISheetPresentationController, medium detent). The page behind the sheet is dimmed, but the white status bar is not."
				},
				new Entry { Placeholder = "Tap here to show the keyboard" },
				new Button
				{
					Text = "Push next page (same StatusBarBehavior)",
					Command = new Command(async () => await Navigation.PushAsync(new MainPage()))
				},
				new Button
				{
					Text = "Open native sheet (medium detent)",
					Command = new Command(PresentNativeMediumSheet)
				},
				new Button
				{
					Text = "Open full screen modal",
					Command = new Command(async () => await Navigation.PushModalAsync(BuildModalPage(UIModalPresentationStyle.FullScreen, Colors.Orange)))
				}
			}
		};
	}

	static void PresentNativeMediumSheet()
	{
		var sheet = new UIKit.UIViewController();
		sheet.View!.BackgroundColor = UIKit.UIColor.SystemBackground;
		sheet.SheetPresentationController!.Detents = [UIKit.UISheetPresentationControllerDetent.CreateMediumDetent()];

		Microsoft.Maui.ApplicationModel.Platform.GetCurrentUIViewController()?.PresentViewController(sheet, true, null);
	}

	static ContentPage BuildModalPage(UIModalPresentationStyle presentationStyle, Color backgroundColor)
	{
		var page = new ContentPage { BackgroundColor = backgroundColor };

		page.Content = new VerticalStackLayout
		{
			Padding = new Thickness(16, 80),
			Spacing = 16,
			Children =
			{
				new Label { Text = $"Modal presented with {presentationStyle}" },
				new Button
				{
					Text = "Close",
					Command = new Command(async () => await page.Navigation.PopModalAsync())
				}
			}
		};

		page.On<iOS>().SetModalPresentationStyle(presentationStyle);

		return page;
	}
}

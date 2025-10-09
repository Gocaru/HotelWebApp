using HotelWebApp.Mobile.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace HotelWebApp.Mobile.Views
{
    public class AboutPage : ContentPage
    {
        public AboutPage(AboutViewModel viewModel)
        {
            Title = "About";
            BindingContext = viewModel;

            // Background com AppThemeBinding direto
            BackgroundColor = Application.Current?.RequestedTheme == AppTheme.Dark
                ? Color.FromArgb("#121212")
                : Color.FromArgb("#F5F5F5");

            Content = CreateContent(viewModel);
        }

        private View CreateContent(AboutViewModel viewModel)
        {
            return new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = new Thickness(20),
                    Spacing = 30,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        // App Icon
                        new Label
                        {
                            Text = "🏨",
                            FontSize = 80,
                            HorizontalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, 60, 0, 40)
                        },

                        // App Name
                        new Label
                        {
                            Text = viewModel.AppName,
                            FontSize = 32,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = GetTextColor(true)
                        },

                        // Information Frame
                        CreateInfoFrame(viewModel),

                        // Technology Frame
                        CreateTechFrame(),

                        // Copyright
                        new Label
                        {
                            Text = $"© 2025 {viewModel.Author}",
                            FontSize = 12,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = GetTextColor(false),
                            Margin = new Thickness(0, 20, 0, 60)
                        }
                    }
                }
            };
        }

        private Frame CreateInfoFrame(AboutViewModel viewModel)
        {
            return new Frame
            {
                CornerRadius = 15,
                Padding = new Thickness(25),
                HasShadow = true,
                BorderColor = Colors.Transparent,
                BackgroundColor = GetCardBackground(),
                Content = new VerticalStackLayout
                {
                    Spacing = 25,
                    Children =
                    {
                        // Author
                        CreateInfoSection("Author", viewModel.Author, Color.FromArgb("#6366F1"), 20, true),
                        CreateSeparator(),

                        // Version
                        CreateInfoSection("Version", viewModel.AppVersion, GetTextColor(true), 18, false),
                        CreateSeparator(),

                        // Release Date
                        CreateInfoSection("Release Date", viewModel.ReleaseDate, GetTextColor(true), 18, false)
                    }
                }
            };
        }

        private VerticalStackLayout CreateInfoSection(string title, string value, Color valueColor, int fontSize, bool isBold)
        {
            return new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new Label
                    {
                        Text = title,
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = GetTextColor(false)
                    },
                    new Label
                    {
                        Text = value,
                        FontSize = fontSize,
                        FontAttributes = isBold ? FontAttributes.Bold : FontAttributes.None,
                        TextColor = valueColor
                    }
                }
            };
        }

        private BoxView CreateSeparator()
        {
            return new BoxView
            {
                HeightRequest = 1,
                Color = GetSeparatorColor()
            };
        }

        private Frame CreateTechFrame()
        {
            var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

            return new Frame
            {
                CornerRadius = 15,
                Padding = new Thickness(20),
                HasShadow = true,
                BorderColor = Colors.Transparent,
                BackgroundColor = isDark ? Color.FromArgb("#1E3A8A") : Color.FromArgb("#EFF6FF"),
                Content = new VerticalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        new Label
                        {
                            Text = "Built with .NET MAUI",
                            FontSize = 14,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = isDark ? Color.FromArgb("#DBEAFE") : Color.FromArgb("#1E3A8A")
                        },
                        new Label
                        {
                            Text = "Cross-platform mobile application",
                            FontSize = 12,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = isDark ? Color.FromArgb("#DBEAFE") : Color.FromArgb("#1E3A8A")
                        }
                    }
                }
            };
        }

        // Helper methods para cores baseadas no tema
        private Color GetTextColor(bool isPrimary)
        {
            var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

            if (isPrimary)
                return isDark ? Colors.White : Color.FromArgb("#1F1F1F");
            else
                return isDark ? Color.FromArgb("#9CA3AF") : Color.FromArgb("#6B7280");
        }

        private Color GetCardBackground()
        {
            var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
            return isDark ? Color.FromArgb("#1F2937") : Colors.White;
        }

        private Color GetSeparatorColor()
        {
            var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
            return isDark ? Color.FromArgb("#374151") : Color.FromArgb("#E5E7EB");
        }
    }
}
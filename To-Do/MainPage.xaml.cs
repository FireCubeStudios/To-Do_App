﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using To_Do.NavigationPages;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI;
//using System.Diagnostics;
using Windows.UI.Xaml.Media;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Security.Authorization.AppCapabilityAccess;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.System.Profile;
using System.Linq;
using Windows.UI.Core;
using Windows.Foundation;

namespace To_Do
{
    public sealed partial class MainPage : Page
    {
        public List<string> savingDescriptions = new List<string>();
        public List<string> savingDates = new List<string>();
        public List<bool> savingImps = new List<bool>();
        public List<string> savingCompletedDates = new List<string>();
        public List<string> completedSaving = new List<string>();
        public List<List<string>> savingSteps = new List<List<string>>();
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static MainPage ins;
        public int indexToParse;
        public List<List<string>> tasksToParse = new List<List<string>>();
        AppCapabilityAccessStatus status;
        public int PendingTasksCount = 0;
        bool canSearch = true;
        public NavigationTransitionInfo info;

        public MainPage()
        {
            this.InitializeComponent();
            ins = this;
            var currentTheme = ThemeHelper.RootTheme.ToString();
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);
            Window.Current.SetTitleBar(AppTitleBar);
            coreTitleBar.LayoutMetricsChanged += (s, a) => UpdateTitleBarLayout(s);
            coreTitleBar.IsVisibleChanged += CoreTitlebar_IsVisibleChanged;
            AppCapability cap = AppCapability.Create("broadFileSystemAccess");
            status = cap.CheckAccess();
            ImageInitialize();
            RoundCornerInitialize();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequest;
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            LoadTheme();
            if (localSettings.Values["navStyle"] != null)
            {
                indexToParse = (int)localSettings.Values["navStyle"];
            }
            else
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
                localSettings.Values["navStyle"] = 0;
                indexToParse = 0;
            }
            currentTheme = ThemeHelper.RootTheme.ToString();
            Waiter();
        }

        public async void Waiter()
        {
            ContentFrame.Navigate(typeof(PendingTasks), null, new SuppressNavigationTransitionInfo());
            ContentFrame.Navigate(typeof(CompletedTasks), null, new SuppressNavigationTransitionInfo());
            await Task.Delay(10);
            ContentFrame.Navigate(typeof(PendingTasks));
            nview.SelectedItem = nview.MenuItems[0];
        }

        public void ImageInitialize()
        {
            if (localSettings.Values["useimg"] != null)
            {
                switch ((int)localSettings.Values["useimg"])
                {
                    case 0:
                        bgIMG.Visibility = Visibility.Collapsed;
                        acrylic.Visibility = Visibility.Collapsed;
                        acrylictint.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        LoadIMG();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                bgIMG.Visibility = Visibility.Collapsed;
                acrylic.Visibility = Visibility.Collapsed;
                acrylictint.Visibility = Visibility.Collapsed;
            }
        }

        public void RoundCornerInitialize()
        {
            if (localSettings.Values["useround"] != null)
            {
                switch ((int)localSettings.Values["useround"])
                {
                    case 0:
                        Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(0);
                        Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(0);
                        Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(1);
                        Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0);
                        Application.Current.Resources["TopLeftNavViewContentCorner"] = new CornerRadius(1, 0, 0, 0);
                        break;
                    case 1:
                        Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(4);
                        Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(8);
                        Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(4);
                        Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0, 8, 8, 0);
                        Application.Current.Resources["TopLeftNavViewContentCorner"] = new CornerRadius(8, 0, 0, 0);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                localSettings.Values["useround"] = 1;
                Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(4);
                Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(8);
                Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(4);
                Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0, 8, 8, 0);
                Application.Current.Resources["TopLeftNavViewContentCorner"] = new CornerRadius(8, 0, 0, 0);
            }
        }

        public void LoadTheme()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            Color fallBackPurple = new Color() { A = 255, R = 103, G = 123, B = 202 };

            if (localSettings.Values["ACCENT1_R"] != null && localSettings.Values["ACCENT2_R"] != null && localSettings.Values["BG_R"] != null)
            {
                byte a1R = (byte)localSettings.Values["ACCENT1_R"];
                byte a1G = (byte)localSettings.Values["ACCENT1_G"];
                byte a1B = (byte)localSettings.Values["ACCENT1_B"];

                byte a2R = (byte)localSettings.Values["ACCENT2_R"];
                byte a2G = (byte)localSettings.Values["ACCENT2_G"];
                byte a2B = (byte)localSettings.Values["ACCENT2_B"];

                byte bgR = (byte)localSettings.Values["BG_R"];
                byte bgG = (byte)localSettings.Values["BG_G"];
                byte bgB = (byte)localSettings.Values["BG_B"];

                Application.Current.Resources["SystemAccentColorDark1"] = new Color() { A = 255, R = a1R, G = a1G, B = a1B };
                Application.Current.Resources["SystemAccentColorDark2"] = new Color() { A = 255, R = a2R, G = a2G, B = a2B };
                Application.Current.Resources["SystemAccentColorLight2"] = (Color)Application.Current.Resources["SystemAccentColorDark2"] == Colors.White ? Application.Current.Resources["SystemAccentColorDark1"] : Application.Current.Resources["SystemAccentColorDark2"];
                Application.Current.Resources["SystemAccentColor"] = ThemeHelper.IsDarkTheme() ? (Color)Application.Current.Resources["SystemAccentColorLight2"] : (Color)Application.Current.Resources["SystemAccentColorDark1"];

                if (ThemeHelper.IsDarkTheme())
                {
                    SolidColorBrush darkbrush = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColorLight2"]);
                    Application.Current.Resources["NavigationViewItemForegroundSelected"] = darkbrush;
                    Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"] = darkbrush;
                    Application.Current.Resources["NavigationViewItemForegroundSelectedPressed"] = darkbrush;
                }

                Color bgColor = new Color() { A = 255, R = bgR, G = bgG, B = bgB };
                Application.Current.Resources["NavigationViewContentBackground"] = new SolidColorBrush(new Color() { A = 150, R = bgR, G = bgG, B = bgB });
                titleBar.ForegroundColor = bgColor;
                titleBar.ButtonHoverBackgroundColor = (Color)Application.Current.Resources["SystemAccentColor"];//ThemeHelper.IsDarkTheme() ? new Color() { A = 255, R = a2R, G = a2G, B = a2B } : bgColor;

                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = ((SolidColorBrush)Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"]).Color;//new Color() { A = 255, R = a2R, G = a2G, B = a2B };
            }
            else
            {
                Application.Current.Resources["SystemAccentColorDark1"] = fallBackPurple;
                Application.Current.Resources["SystemAccentColorDark2"] = Colors.White;
                Application.Current.Resources["SystemAccentColorLight2"] = (Color)Application.Current.Resources["SystemAccentColorDark2"] == Colors.White ? Application.Current.Resources["SystemAccentColorDark1"] : Application.Current.Resources["SystemAccentColorDark2"];
                Application.Current.Resources["SystemAccentColor"] = ThemeHelper.IsDarkTheme() ? (Color)Application.Current.Resources["SystemAccentColorLight2"] : (Color)Application.Current.Resources["SystemAccentColorDark1"];

                Application.Current.Resources["NavigationViewItemForegroundSelected"] = Application.Current.Resources["SystemAccentColor"];
                Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"] = Application.Current.Resources["SystemAccentColor"];
                Application.Current.Resources["NavigationViewItemForegroundSelectedPressed"] = Application.Current.Resources["SystemAccentColor"];

                Color bgColor = fallBackPurple;
                Application.Current.Resources["NavigationViewContentBackground"] = new SolidColorBrush(new Color() { A = 150, R = fallBackPurple.R, G = fallBackPurple.G, B = fallBackPurple.B });
                titleBar.ForegroundColor = bgColor;
                titleBar.ButtonHoverBackgroundColor = bgColor;
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = fallBackPurple;
            }
        }

        async void LoadIMG()
        {
            if (localSettings.Values["imgPath"] != null)
            {
                if (status == AppCapabilityAccessStatus.DeniedByUser)
                {
                    bgIMG.Visibility = Visibility.Collapsed;
                    acrylic.Visibility = Visibility.Collapsed;
                    acrylictint.Visibility = Visibility.Collapsed;
                }
                else
                {
                    string path = (string)localSettings.Values["imgPath"];
                    StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                    if (file != null)
                    {
                        bgIMG.Visibility = Visibility.Visible;
                        acrylic.Visibility = Visibility.Visible;
                        acrylictint.Visibility = Visibility.Visible;
                        using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                        {
                            // Set the image source to the selected bitmap
                            BitmapImage bitmapImage = new BitmapImage();

                            await bitmapImage.SetSourceAsync(fileStream);
                            bgIMG.Source = bitmapImage;
                        }
                    }
                    else
                    {
                        bgIMG.Visibility = Visibility.Collapsed;
                        acrylic.Visibility = Visibility.Collapsed;
                        acrylictint.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                bgIMG.Visibility = Visibility.Collapsed;
                acrylic.Visibility = Visibility.Collapsed;
                acrylictint.Visibility = Visibility.Collapsed;
            }
        }

        private void CoreTitlebar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string argument = e.Parameter.ToString();
            if (argument == "GoToPending")
            {
                ContentFrame.Navigate(typeof(PendingTasks), null, new SuppressNavigationTransitionInfo());
                //await Task.Delay(10);
                ContentFrame.Navigate(typeof(CompletedTasks), null, new SuppressNavigationTransitionInfo());
                await Task.Delay(10);
                ContentFrame.Navigate(typeof(PendingTasks));
                nview.SelectedItem = nview.MenuItems[0];
            }
            else if (argument == "GoToCompleted")
            {
                ContentFrame.Navigate(typeof(PendingTasks), null, new SuppressNavigationTransitionInfo());
                await Task.Delay(10);
                ContentFrame.Navigate(typeof(CompletedTasks), null, new SuppressNavigationTransitionInfo());
                nview.SelectedItem = nview.MenuItems[1];
            }
            else if (argument == "GoToSettings")
            {
                ContentFrame.Navigate(typeof(PendingTasks), null, new SuppressNavigationTransitionInfo());
                ContentFrame.Navigate(typeof(CompletedTasks), null, new SuppressNavigationTransitionInfo());
                //await Task.Delay(10);
                await Task.Delay(10);
                ContentFrame.Navigate(typeof(Settings), null, new SuppressNavigationTransitionInfo());
                nview.SelectedItem = nview.SettingsItem;
            }
        }

        public void CreateThreeTileNotifications()
        {
            var TaskItems = PendingTasks.instance.TaskItems;
            if (TaskItems.Count > 0)
            {
                List<string> tasks = new List<string>();
                string title = "Tasks to do";
                for (int i = 0; i < TaskItems.Count; i++)
                {
                    tasks.Add(TaskItems[i].Description);
                }
                int amountOfTasks = TaskItems.Count;

                TileContent content = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        Branding = TileBranding.None,
                        TileSmall = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = TaskItems.Count.ToString(),
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.Caption
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "Tasks",
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                }
                            }
                        },
                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children = {
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Base,
                                },

                                new AdaptiveText()
                                {
                                    Text = "1) " + tasks[0],
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {

                                    Text = tasks.Count > 1 ? "2) " + tasks[1] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {

                                    Text = tasks.Count > 2 ? "3) " + tasks[2] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 3 ? $"+{amountOfTasks - 3} task(s)" : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                            }
                            }
                        },

                        TileWide = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children = {
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Base
                                },

                                new AdaptiveText()
                                {
                                    Text = "1) " + tasks[0],
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 1 ? "2) " + tasks[1] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 2 ? "3) " + tasks[2] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 3 ? $"+{amountOfTasks - 3} task(s)" : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                            }
                        },
                        TileLarge = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children = {
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Base
                                },

                                new AdaptiveText()
                                {
                                    Text = "1) " + tasks[0],
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 1 ? "2) " + tasks[1] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 2 ? "3) " + tasks[2] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 3 ? "4) " + tasks[3] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 4 ? "5) " + tasks[4] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 5 ? "6) " + tasks[5] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 6 ? "7) " + tasks[6] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 7 ? "8) " + tasks[7] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 8 ? "9) " + tasks[8] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 9 ? $"+{amountOfTasks - 9} task(s)" : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                            }
                            }
                        }
                    }
                };
                var notif = new TileNotification(content.GetXml());
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notif);
            }
            else
            {
                //all tasks complete
                TileContent content = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        Branding = TileBranding.None,
                        TileSmall = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children =
                                {
                                    new AdaptiveGroup()
                                    {
                                        Children =
                                        {
                                            new AdaptiveSubgroup()
                                            {
                                                Children =
                                                {
                                                    new AdaptiveImage()
                                                    {
                                                        Source = "Images/tileIcon.png",
                                                        HintAlign = AdaptiveImageAlign.Center
                                                    },
                                                }
                                            },
                                        }
                                    },
                                    new AdaptiveGroup()
                                    {
                                        Children =
                                        {
                                            new AdaptiveSubgroup()
                                            {
                                                Children =
                                                {
                                                    new AdaptiveText()
                                                    {
                                                        Text = "Done!",
                                                        HintAlign = AdaptiveTextAlign.Center,
                                                        HintWrap = true,
                                                        HintMaxLines = 2,
                                                        HintStyle = AdaptiveTextStyle.Base
                                                    }
                                                }
                                            },
                                        }
                                    },
                                }
                            }
                        },
                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children = {
                                    new AdaptiveImage()
                                    {
                                        Source = "Images/tileIcon.png",
                                        HintAlign = AdaptiveImageAlign.Center
                                    },

                                    new AdaptiveText()
                                    {
                                        Text = "All Done!",
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintWrap = true,
                                        HintMaxLines = 2,
                                        HintStyle = AdaptiveTextStyle.Base
                                    }
                                }
                            }
                        },

                        TileWide = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children = {

                                    new AdaptiveImage()
                                    {
                                        Source = "Images/tileIcon.png",
                                        HintAlign = AdaptiveImageAlign.Center
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "All Tasks Completed!",
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.Base
                                    }
                                }
                            }
                        },
                        TileLarge = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children = {
                                    new AdaptiveImage()
                                    {
                                        Source = "Images/tileIcon.png",
                                        HintAlign = AdaptiveImageAlign.Center
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "All Tasks Completed!",
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.Base
                                    }
                                }
                            }
                        }
                    }
                };
                var notif = new TileNotification(content.GetXml())
                {
                    ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(1)
                };
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notif);
            }
        }

        public void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            PendingTasks ins = PendingTasks.instance;
            //save all descriptions
            foreach (TODOTask tODO in ins.TaskItems)
            {
                string temp = tODO.Description;
                string date = tODO.Date;
                bool importance = tODO.IsStarred;
                savingDescriptions.Add(temp);
                savingDates.Add(date);
                savingImps.Add(importance);

                List<TODOTask> steps = tODO.SubTasks;
                List<string> tempList = new List<string>();
                for (int i = 0; i < steps.Count; i++)
                {
                    tempList.Add(steps[i].Description);
                }
                if (steps != null)
                {
                    savingSteps.Add(tempList);
                }
            }
            Type pageType = Type.GetType("To_Do.NavigationPages.CompletedTasks");
            ContentFrame.Navigate(pageType, tasksToParse, new SuppressNavigationTransitionInfo());
            tasksToParse.Clear();
            foreach (TODOTask task in CompletedTasks.instance.CompleteTasks)
            {
                string temp = task.Description;
                string date = task.Date;
                completedSaving.Add(temp);
                savingCompletedDates.Add(date);
            }
            string jsonFile = JsonConvert.SerializeObject(savingDescriptions);
            string compJsonFile = JsonConvert.SerializeObject(completedSaving);
            string dateJsonFile = JsonConvert.SerializeObject(savingDates);
            string compdateJsonFile = JsonConvert.SerializeObject(savingCompletedDates);
            string importanceJsonFile = JsonConvert.SerializeObject(savingImps);
            string stepsJsonFile = JsonConvert.SerializeObject(savingSteps);
            localSettings.Values["Tasks"] = jsonFile;
            localSettings.Values["TasksDone"] = compJsonFile;
            localSettings.Values["DateOfTasks"] = dateJsonFile;
            localSettings.Values["DateOfTasksDone"] = compdateJsonFile;
            localSettings.Values["ImportanceOfTasks"] = importanceJsonFile;
            localSettings.Values["Steps"] = stepsJsonFile;

            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong build = (version & 0x00000000FFFF0000L) >> 16;

            if (Convert.ToInt16(build) < 22000)
            {
                CreateThreeTileNotifications();
            }

            //// Get the blank badge XML payload for a badge number
            //XmlDocument badgeXml =
            //    BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            //// Set the value of the badge in the XML to our number
            //XmlElement badgeElement = badgeXml.SelectSingleNode("/badge") as XmlElement;
            //badgeElement.SetAttribute("value", "7");

            //// Create the badge notification
            //BadgeNotification badge = new BadgeNotification(badgeXml);

            //// Create the badge updater for the application
            //BadgeUpdater badgeUpdater =
            //    BadgeUpdateManager.CreateBadgeUpdaterForApplication();

            //// And update the badge
            //badgeUpdater.Update(badge);
        }


        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            AppTitleBar.Height = coreTitleBar.Height;

            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            LoadingUI.Visibility = Visibility.Visible;
            int styleIndex = (int)localSettings.Values["navStyle"];
            switch (styleIndex)
            {
                case 0:
                    info = new EntranceNavigationTransitionInfo();
                    break;
                case 1:
                    info = new DrillInNavigationTransitionInfo();
                    break;
                case 2:
                    info = new SuppressNavigationTransitionInfo();
                    break;
                case 3:
                    info = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft };
                    break;
                case 4:
                    info = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };
                    break;
                default:
                    info = new EntranceNavigationTransitionInfo();
                    break;
            }
            Type pageType;
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            if (args.IsSettingsSelected)
            {
                pageType = Type.GetType("To_Do.Settings");
                ContentFrame.Navigate(pageType, null, info);
            }
            else
            {
                string selectedItemTag = selectedItem.Tag.ToString();
                string pageName = "To_Do.NavigationPages." + selectedItemTag;
                pageType = Type.GetType(pageName);
                switch (selectedItemTag)
                {
                    case "CompletedTasks":
                        ContentFrame.Navigate(pageType, tasksToParse, info);
                        tasksToParse.Clear();
                        break;
                    case "PendingTasks":
                        ContentFrame.Navigate(pageType, null, info);
                        break;
                    default:
                        break;
                }
            }
            LoadingUI.Visibility = Visibility.Collapsed;
        }

        private void nview_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 48;

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (sender.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                bgIMG.Margin = new Thickness(0, -60, 0, 0);
                acrylic.Margin = new Thickness(-4, -61, -4, -4);
                acrylictint.Margin = new Thickness(-4, -61, -4, -4);
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                bgIMG.Margin = new Thickness(0, -82, 0, 0);
                acrylic.Margin = new Thickness(-4, -84, -4, -4);
                acrylictint.Margin = new Thickness(-4, -84, -4, -4);
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }

        private List<QueryFormat> SearchControls(string query)
        {
            var suggestions = new List<QueryFormat>();
            var querySplit = query.Split(" ");

            if (PendingTasks.instance != null)
            {
                var matchingItems = PendingTasks.instance.TaskItems.ToList().Where(
                    item =>
                    {
                        // Idea: check for every word entered (separated by space) if it is in the name,  
                        // e.g. for query "split button" the only result should "SplitButton" since its the only query to contain "split" and "button" 
                        // If any of the sub tokens is not in the string, we ignore the item. So the search gets more precise with more words 
                        bool flag = true;
                        foreach (string queryToken in querySplit)
                        {
                            // Check if token is not in string 
                            if (item.Description.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                // Token is not in string, so we ignore this item. 
                                flag = false;
                            }
                        }
                        return flag;
                    });

                foreach (var item in matchingItems)
                {
                    suggestions.Add(new QueryFormat(item.Description, "Pending Tasks", "\uE823"));
                }

            }

            if (CompletedTasks.instance != null)
            {

                var matchingItems2 = CompletedTasks.instance.CompleteTasks.ToList().Where(
                    item =>
                    {
                        // Idea: check for every word entered (separated by space) if it is in the name,  
                        // e.g. for query "split button" the only result should "SplitButton" since its the only query to contain "split" and "button" 
                        // If any of the sub tokens is not in the string, we ignore the item. So the search gets more precise with more words 
                        bool flag = true;
                        foreach (string queryToken in querySplit)
                        {
                            // Check if token is not in string 
                            if (item.Description.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                // Token is not in string, so we ignore this item. 
                                flag = false;
                            }
                        }
                        return flag;
                    });
                foreach (var item in matchingItems2)
                {
                    suggestions.Add(new QueryFormat(item.Description, "Completed Tasks", "\uE73E"));
                }
            }

            return suggestions.OrderByDescending(i => i.Title.StartsWith(query, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Title).ToList();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (canSearch)
            {
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    var suggestions = SearchControls(sender.Text);

                    if (suggestions.Count > 0)
                    {
                        sender.ItemsSource = suggestions;
                    }
                    else
                    {
                        sender.ItemsSource = new List<QueryFormat> { new QueryFormat("No result found", "Everywhere", "\uE711") };
                    }
                }
            }
        }

        private async void SelectControl(QueryFormat desc)
        {
            if (desc != null && desc.location != "Everywhere")
            {
                if (desc.location == "Pending Tasks")
                {
                    var list = PendingTasks.instance.TaskItems;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (desc.Title.Equals(list[i].Description))
                        {
                            ContentFrame.Navigate(typeof(PendingTasks), null, info);
                            nview.SelectedItem = nview.MenuItems[0];
                            //await Task.Delay(500);
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                () => calc(i, PendingTasks.instance.listOfTasks));
                            break;
                        }
                    }
                }
                else
                {
                    var list = CompletedTasks.instance.CompleteTasks;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (desc.Title.Equals(list[i].Description))
                        {
                            ContentFrame.Navigate(typeof(CompletedTasks), null, info);
                            nview.SelectedItem = nview.MenuItems[1];
                            //await Task.Delay(500);
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                () => calc(i, CompletedTasks.instance.listOfTasks));
                            break;
                        }
                    }
                }
            }
        }

        public async void calc(int index, ListView choice)
        {
            await ScrollToIndex(choice, index);
        }

        public ScrollViewer GetScrollViewer(DependencyObject element)
        {
            if (element is ScrollViewer viewer)
            {
                return viewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }

        public async Task ScrollToIndex(ListView listViewBase, int index)
        {
            bool isVirtualizing = default(bool);
            double previousHorizontalOffset = default(double), previousVerticalOffset = default(double);

            // get the ScrollViewer withtin the ListView/GridView
            var scrollViewer = GetScrollViewer(listViewBase);
            if (scrollViewer != null)
            {
                // get the SelectorItem to scroll to
                var selectorItem = listViewBase.ContainerFromIndex(index) as ListViewItem;

                // when it's null, means virtualization is on and the item hasn't been realized yet
                if (selectorItem == null)
                {
                    isVirtualizing = true;

                    previousHorizontalOffset = scrollViewer.HorizontalOffset;
                    previousVerticalOffset = scrollViewer.VerticalOffset;

                    // call task-based ScrollIntoViewAsync to realize the item
                    await ScrollIntoViewAsync(listViewBase, listViewBase.Items[index]);

                    // this time the item shouldn't be null again
                    selectorItem = (ListViewItem)listViewBase.ContainerFromIndex(index);
                }

                // calculate the position object in order to know how much to scroll to
                var transform = selectorItem.TransformToVisual((UIElement)scrollViewer.Content);
                var position = transform.TransformPoint(new Point(0, 0));

                // when virtualized, scroll back to previous position without animation
                if (isVirtualizing)
                {
                    await ChangeViewAsync(scrollViewer, previousHorizontalOffset, previousVerticalOffset, true);
                }

                // scroll to desired position with animation!
                scrollViewer.ChangeView(position.X, position.Y, null);
            }

        }

        public async Task ScrollIntoViewAsync(ListView listViewBase, object item)
        {
            var tcs = new TaskCompletionSource<object>();
            var scrollViewer = GetScrollViewer(listViewBase);

            EventHandler<ScrollViewerViewChangedEventArgs> viewChanged = (s, e) => tcs.TrySetResult(null);
            try
            {
                scrollViewer.ViewChanged += viewChanged;
                listViewBase.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
                await tcs.Task;
            }
            finally
            {
                scrollViewer.ViewChanged -= viewChanged;
            }
        }

        public async Task ChangeViewAsync(ScrollViewer scrollViewer, double? horizontalOffset, double? verticalOffset, bool disableAnimation)
        {
            var tcs = new TaskCompletionSource<object>();

            EventHandler<ScrollViewerViewChangedEventArgs> viewChanged = (s, e) => tcs.TrySetResult(null);
            try
            {
                scrollViewer.ViewChanged += viewChanged;
                scrollViewer.ChangeView(horizontalOffset, verticalOffset, null, disableAnimation);
                await tcs.Task;
            }
            finally
            {
                scrollViewer.ViewChanged -= viewChanged;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is QueryFormat)
            {
                //User selected an item, take an action
                SelectControl(args.ChosenSuggestion as QueryFormat);
            }
            else if (!string.IsNullOrEmpty(args.QueryText))
            {
                //Do a fuzzy search based on the text
                var suggestions = SearchControls(sender.Text);
                if (suggestions.Count > 0)
                {
                    SelectControl(suggestions.FirstOrDefault());
                }
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is QueryFormat control)
            {
                if (control.Title != "To_Do.QueryFormat")
                {
                    sender.Text = control.Title;

                }
            }
        }

        public async void Refresh()
        {
            LoadingUI.Visibility = Visibility.Visible;
            ContentFrame.Navigate(typeof(CompletedTasks), tasksToParse, new SuppressNavigationTransitionInfo());
            tasksToParse.Clear();
            await Task.Delay(10);
            ContentFrame.Navigate(typeof(PendingTasks), null, new SuppressNavigationTransitionInfo());
            nview.SelectedItem = nview.MenuItems[0];
            LoadingUI.Visibility = Visibility.Collapsed;
        }
    }

    public class QueryFormat
    {
        public string Title = "";
        public string location = "";
        public string glyph = "";

        public QueryFormat(string _title, string _location, string _glyph)
        {
            Title = _title;
            location = _location;
            glyph = _glyph;
        }
    }
}

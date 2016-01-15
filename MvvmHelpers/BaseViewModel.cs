namespace MvvmHelpers
{
    /// <summary>
    /// Base view model.
    /// </summary>
    public class BaseViewModel : ObservableObject
    {
        string title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title {
            get { return title; } 
            set 
            { 
                if (title == value)
                    return;
                title = value;
                OnPropertyChanged();
            }
        }

        string subtitle;
        /// <summary>
        /// Gets or sets the subtitle.
        /// </summary>
        /// <value>The subtitle.</value>
        public string Subtitle {
            get { return subtitle; } 
            set 
            { 
                if (subtitle == value)
                    return;
                subtitle = value;
                OnPropertyChanged();
            }
        }

        string icon;
        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public string Icon {
            get { return icon; } 
            set 
            { 
                if (icon == value)
                    return;
                icon = value;
                OnPropertyChanged();
            }
        }

        bool isBusy;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get { return isBusy; } 
            set
            { 
                if (isBusy == value)
                    return;
                isBusy = value;
                IsNotBusy = !isBusy;
                OnPropertyChanged();
            }
        }

        bool isNotBusy;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is not busy.
        /// </summary>
        /// <value><c>true</c> if this instance is not busy; otherwise, <c>false</c>.</value>
        public bool IsNotBusy {
            get { return isNotBusy; } 
            set 
            { 
                if (isNotBusy == value)
                    return;
                isNotBusy = value;
                isBusy = !isNotBusy;
                OnPropertyChanged();
            }
        }

        bool canLoadMore;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can load more.
        /// </summary>
        /// <value><c>true</c> if this instance can load more; otherwise, <c>false</c>.</value>
        public bool CanLoadMore
        {
            get { return canLoadMore; } 
            set
            { 
                if (canLoadMore == value)
                    return;
                canLoadMore = value;
                OnPropertyChanged();
            }
        }
    }
}



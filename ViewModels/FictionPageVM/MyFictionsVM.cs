using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using TaskTip.Services;
        {
            get=> _descVisibility;
            set=> SetProperty(ref _descVisibility, value);
        }
            {
                DescVisibility = _fictionItem.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
                    Directory.Delete(Path.Combine(GlobalVariable.FictionCachePath, model.Id), true);


                }
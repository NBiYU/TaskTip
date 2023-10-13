﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Configuration;
using System.Windows.Input;
using TaskTip.Services;
using Application = System.Windows.Application;

namespace TaskTip.ViewModels.UserViewModel
{
    public class TaskBarIconViewModel : ObservableObject
    {
        /// <summary>
        /// 显示悬浮窗
        /// </summary>
        public ICommand ShowFloating
        {
            get
            {
                return new Lazy<RelayCommand>(() => new RelayCommand(() =>
                {
                    if (GlobalVariable.IsFloatingImageStyle)
                    {
                        GlobalVariable.FloatingViewShow();
                    }
                    else
                    {
                        GlobalVariable.FloatingTitleStyleViewShow();
                    }
                })).Value;
            }
        }

        /// <summary>
        /// 隐藏悬浮窗
        /// </summary>
        public ICommand HideCommand
        {
            get
            {
                return new Lazy<RelayCommand>(() => new RelayCommand(() =>
                {
                    if (GlobalVariable.IsFloatingImageStyle)
                    {
                        GlobalVariable.FloatingViewHide();
                    }
                    else
                    {
                        GlobalVariable.FloatingTitleStyleViewHide();
                    }
                })).Value;
            }
        }
        /// <summary>
        /// 退出程序
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                return new Lazy<RelayCommand>(() => new RelayCommand(() =>
                {
                    Application.Current.Shutdown();
                })).Value;
            }
        }
        /// <summary>
        /// 显示设置界面
        /// </summary>
        public ICommand ShowCustomCommand
        {
            get
            {
                return new Lazy<RelayCommand>(() => new RelayCommand(() =>
                {
                    GlobalVariable.CustomSetViewShow();
                })).Value;
            }
        }
    }
}

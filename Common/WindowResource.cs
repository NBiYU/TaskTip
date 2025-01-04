using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TaskTip.Pages;
using TaskTip.Services;
using TaskTip.Views.Windows;
using TaskTip.Views;
using System.Windows;
using TaskTip.Models.Enums;

namespace TaskTip.Common
{
    public static class WindowResource
    {

        public static double Left { get; set; } = SystemParameters.WorkArea.Height * 0.3;
        public static double Top { get; set; } = SystemParameters.WorkArea.Width * 0.3;

        #region 窗口显示状态管理


        #region FloatingWindow

        public static void SwitchFloatingShow(FloatingStyleEnum styleEnum)
        {
            if (styleEnum == FloatingStyleEnum.Image)
            {
                FloatingViewShow();
            }
            else if (styleEnum == FloatingStyleEnum.Title)
            {
                FloatingTitleStyleViewShow();
            }
            else if (styleEnum == FloatingStyleEnum.Status)
            {
                SysRuntimeStatusViewShow();
            }
            else if (styleEnum == FloatingStyleEnum.Task)
            {
                FloatingTaskViewShow();
            }
        }
        public static void FloatingClose(FloatingStyleEnum styleEnum)
        {
            if (styleEnum == FloatingStyleEnum.Image)
            {
                FloatingViewClose();
            }
            else if (styleEnum == FloatingStyleEnum.Title)
            {
                FloatingTitleStyleViewClose();
            }
            else if (styleEnum == FloatingStyleEnum.Status)
            {
                SysRuntimeStatusViewClose();
            }
            else if (styleEnum == FloatingStyleEnum.Task)
            {
                FloatingTaskViewClose();
            }
        }
        public static void FlotingHidden(FloatingStyleEnum styleEnum)
        {
            if (styleEnum == FloatingStyleEnum.Image)
            {
                FloatingViewHide();
            }
            else if (styleEnum == FloatingStyleEnum.Title)
            {
                FloatingTitleStyleViewHide();
            }
            else if (styleEnum == FloatingStyleEnum.Status)
            {
                SysRuntimeStatusViewHide();
            }
            else if (styleEnum == FloatingStyleEnum.Task)
            {
                FloatingTaskViewHide();
            }
        }
        public static void FloatingCloseAll()
        {
            FloatingViewClose();
            FloatingTitleStyleViewClose();
            SysRuntimeStatusViewClose();
            FloatingTaskViewClose();
        }


        #region Floating
        public static FloatingView FloatingView { get; set; } = new();
        public static void FloatingViewShow(string imagePath = "")
        {

            if (FloatingView.IsClosed)
            {
                FloatingView = new FloatingView();
            }
            if (!string.IsNullOrEmpty(imagePath))
            {
                FloatingView.FloatingBgImage.Source = new BitmapImage(new Uri(imagePath));
            }
            FloatingView.Show();
        }

        public static void FloatingViewHide()
        {
            if (FloatingView.IsClosed)
            {
                FloatingView = new FloatingView();
            }
            FloatingView.Hide();
        }

        public static void FloatingViewClose()
        {
            if (FloatingView != null && !FloatingView.IsClosed)
            {
                FloatingView.Close();
            }
        }

        #endregion

        #region FloatingTitleStyle
        public static FloatingTitleStyleView FloatingTitleStyleView { get; set; } = new();
        public static void FloatingTitleStyleViewShow()
        {
            if (FloatingTitleStyleView.IsClosed)
            {
                FloatingTitleStyleView = new FloatingTitleStyleView();
            }
            FloatingTitleStyleView.Show();
        }

        public static void FloatingTitleStyleViewHide()
        {
            if (FloatingTitleStyleView.IsClosed)
            {
                FloatingTitleStyleView = new FloatingTitleStyleView();
            }
            FloatingTitleStyleView.Hide();
        }

        public static void FloatingTitleStyleViewClose()
        {
            if (FloatingTitleStyleView != null && !FloatingTitleStyleView.IsClosed)
            {
                FloatingTitleStyleView.Close();
            }
        }
        #endregion

        #region FloatingStatusStyle
        public static SysRuntimeStatusView SysRuntimeStatusView { get; set; } = new();
        public static void SysRuntimeStatusViewShow()
        {
            if (SysRuntimeStatusView.IsClosed)
            {
                SysRuntimeStatusView = new();
            }
            SysRuntimeStatusView.Show();
        }

        public static void SysRuntimeStatusViewHide()
        {
            if (SysRuntimeStatusView.IsClosed)
            {
                SysRuntimeStatusView = new();
            }
            SysRuntimeStatusView.Hide();
        }

        public static void SysRuntimeStatusViewClose()
        {
            if (SysRuntimeStatusView != null && !SysRuntimeStatusView.IsClosed)
            {
                SysRuntimeStatusView.Close();
            }
        }

        #endregion

        #region FloatingTaskStyle

        private static FloatingTaskView FloatingTaskView = new();
        public static void FloatingTaskViewShow()
        {
            if (FloatingTaskView.IsClosed)
            {
                FloatingTaskView = new();
            }
            FloatingTaskView.Show();
        }

        public static void FloatingTaskViewHide()
        {
            if (FloatingTaskView.IsClosed)
            {
                FloatingTaskView = new();
            }
            FloatingTaskView.Hide();
        }

        public static void FloatingTaskViewClose()
        {
            if (FloatingTaskView != null && !FloatingTaskView.IsClosed)
            {
                FloatingTaskView.Close();
            }
        }

        #endregion

        #endregion

        #region TaskMeno
        public static TaskMenoView TaskMenoView { get; set; } = new();
        public static void TaskMenoViewShow()
        {
            if (TaskMenoView.IsClosed)
            {
                TaskMenoView = new TaskMenoView();
            }
            TaskMenoView.Show();
        }
        public static void TaskMenoViewHide()
        {

            if (TaskMenoView.IsClosed)
            {
                TaskMenoView = new TaskMenoView();
            }

            Left = TaskMenoView.Left + TaskMenoView.Width;
            Top = TaskMenoView.Top + TaskMenoView.Height;
            TaskMenoView.Hide();
        }

        public static void TaskMenoViewClose()
        {
            Left = TaskMenoView.Left + TaskMenoView.Width;
            Top = TaskMenoView.Top + TaskMenoView.Height;
            if (TaskMenoView != null && !TaskMenoView.IsClosed)
            {
                TaskMenoView.Close();
            }
        }

        #endregion

        #region CustomSet
        public static CustomSetView CustomSetView { get; set; } = new();
        public static void CustomSetViewShow()
        {
            if (CustomSetView.IsClosed)
            {
                CustomSetView = new CustomSetView();
            }
            CustomSetView.Show();
        }

        public static void CustomSetViewHide()
        {
            if (CustomSetView.IsClosed)
            {
                CustomSetView = new CustomSetView();
            }
            Left = CustomSetView.Left + CustomSetView.Width;
            Top = CustomSetView.Top + CustomSetView.Height;
            CustomSetView.Hide();
        }

        public static void CustomSetViewClose()
        {
            Left = CustomSetView.Left + CustomSetView.Width;
            Top = CustomSetView.Top + CustomSetView.Height;
            if (CustomSetView != null && !CustomSetView.IsClosed)
            {
                CustomSetView.Close();
            }
        }

        #endregion

        #region EditFullScreenView
        public static EditFullScreenView EditFullScreenView { get; set; } = new();
        public static void EditFullScreenViewShow()
        {
            if (EditFullScreenView.IsClosed)
            {
                EditFullScreenView = new EditFullScreenView();
            }
            EditFullScreenView.Show();
        }

        public static void EditFullScreenViewHide()
        {
            if (EditFullScreenView.IsClosed)
            {
                EditFullScreenView = new EditFullScreenView();
            }
            EditFullScreenView.Hide();
        }

        public static void EditFullScreenViewClose()
        {
            if (EditFullScreenView != null && !EditFullScreenView.IsClosed)
            {
                EditFullScreenView.Close();
            }
        }

        #endregion

        #endregion

        #region 静态页面

        public static RecordPage RecordPage = new();

        #endregion

    }
}

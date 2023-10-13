using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TaskTip.Common;
        {
            get => _currentProgress;
            set=>SetProperty(ref _currentProgress, value);
        }
        {
            get=> _loadingVisibility;
            set=>SetProperty(ref _loadingVisibility, value);
        }
                // ������С��ʱ���߶Ȼ��Ϊ160
                if (value > 160)
                {
                    if (value < 200)
                    {
                        MessageBox.Show("���ȹ�С");
                        return;
                    }
                    SetProperty(ref _windowWidth, value);
                    SaveConfig<double>(value, nameof(WindowWidth));
                    DirectoryWidth = value * 0.4;
                    SettingWidth = value;
                }

                {
                    if (value < 100)
                    {
                        MessageBox.Show("�߶ȹ�С");
                        return;
                    }
                    SetProperty(ref _windowHeight, value);
                    SaveConfig<double>(value, nameof(WindowHeight));
                    DirectoryHeight = value * 0.8;
                    SettingHeight = value * 0.4;
                }

            }



        #endregion
        #region ���ܺ���
        {
            //int total = _currentChapterContent.Count/ContentGap ;
            //int idx = _model.LastContentReadIndex / ContentGap + (_model.LastContentReadIndex % ContentGap > 0 ? 1 : 0);
            var remainderNum = _currentChapterContent.Count - _model.LastContentReadIndex;
            var currentShowNum = remainderNum > ContentGap ? ContentGap : remainderNum;
            var progress = (int)(((_model.LastContentReadIndex + currentShowNum) * 1.0 / _currentChapterContent.Count) * 100);
            CurrentProgress = $"{progress}%";
        }

            if (LoadingVisibility == Visibility.Visible) return;
            {
                var selectChapter = _chapterInfo.chapterList[_model.LastChapterReadIndex];

                var bookCacheDirPath = Path.Combine(GlobalVariable.FictionCachePath, _model.Id, "ChapterCache");//��Ӧ�鼮����·��
                var bookChapterCachePath = Path.Combine(bookCacheDirPath, selectChapter.chapterId) + ".txt";

                // ���ػ���
                if (File.Exists(bookChapterCachePath))
                {
                    var cacheContent = await File.ReadAllTextAsync(bookChapterCachePath);
                    _currentChapterContent = cacheContent.Split("\n&&\n").ToList();
                }
                else
                {
                    var content = await _request.SearchContent(selectChapter.chapterId);
                    _currentChapterContent = new List<string>(content.data);
                    if (!Directory.Exists(bookCacheDirPath)) Directory.CreateDirectory(bookCacheDirPath);
                    await File.WriteAllTextAsync(bookChapterCachePath, string.Join("\n&&\n", _currentChapterContent)); // �����鼮�½���Ϣ
                }
                _currentChapterContent.RemoveAll(x=>string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));
                await LoadedContent(_model.LastContentReadIndex, ContentGap);
            }
            catch(Exception e)
            {
                MessageBox.Show($"�ڼ��ع�������������{e}");
            }
            finally
            {
                LoadingVisibility = Visibility.Collapsed;
            }





        #endregion
        #region ��ʼ��
        private void SettingValue()
        
        #endregion
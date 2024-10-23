using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskTip.Common;
using TaskTip.Models.DataModel;
using TaskTip.Views.Windows.PopWindow;

namespace TaskTip.ViewModels.ToolPageVM
{
    public partial class JsonVM : ObservableObject
    {

        private readonly static string _stringTip = "切换到实体视图";
        private readonly static string _entityTip = "切换到JSON视图";

        #region 属性

        private bool _isString = true;
        public  bool IsString { get => _isString; 
            set {
                if (value)
                {
                    TransformTip = _stringTip;
                    StringVisibility = Visibility.Visible;
                    EntityVisibility = Visibility.Collapsed;
                }
                else
                {
                    TransformTip = _entityTip;
                    StringVisibility = Visibility.Collapsed;
                    EntityVisibility = Visibility.Visible;
                }
                _isString = value;
            } 
        }

        private Visibility _entityVisibility = Visibility.Collapsed;
        private Visibility _stringVisibility = Visibility.Visible;
        private string _transformTip = _stringTip;
        private int _gridColumnId = 0;
        private string _jsonString = string.Empty;
        private JsonEntityModel _entityModel = new();
        private string _errMsg;
        public Visibility EntityVisibility {  get => _entityVisibility; set=>SetProperty(ref _entityVisibility, value);}
        public Visibility StringVisibility {  get => _stringVisibility; set=>SetProperty(ref _stringVisibility, value);}
        public string TransformTip { get => _transformTip; set => SetProperty(ref _transformTip, value); }
        public int GridColumnId { get => _gridColumnId; set=>SetProperty(ref _gridColumnId, value); }
        public string JsonString { get => _jsonString;set=>SetProperty(ref _jsonString, value); }
        public JsonEntityModel EntityModel { get => _entityModel; set => SetProperty(ref _entityModel, value); }
        public string ErrMsg { get => _errMsg; set => SetProperty(ref _errMsg, value); }
        #endregion 属性

        #region 指令
        [RelayCommand]
        public void JsonFormat()
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<JObject>(JsonString);
                if (obj == null) return;
                JsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        [RelayCommand]
        public async Task JsonOrEntity()
        {
            try
            {
                IsString = !IsString;
                if (!IsString)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(JsonString);
                    if (obj == null) { EntityModel = new JsonEntityModel(new JObject()) ; return; };
                    var model = new JsonEntityModel();
                    foreach (var children in obj.Children().Values())
                    {
                        model.SubsetList.Add(await GetBaseTypeEntity(children));
                    }
                    EntityModel = model;           
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        [RelayCommand]
        public async Task ScreenFull()
        {
            var window = new JsonFullEditWidnow(this);
            window.Show();
        }

        #endregion 指令

        #region 处理函数
        public async Task JsonStringToEntity()
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<JObject>(JsonString);
                if (obj == null) { EntityModel = new JsonEntityModel(new JObject()); return; };
                var model = new JsonEntityModel();
                foreach (var children in obj.Children().Values())
                { 
                    model.SubsetList.Add(await GetBaseTypeEntity(children));
                }
                EntityModel = model;
                //JsonString = JsonConvert.SerializeObject(obj,Formatting.Indented);
                ErrMsg = string.Empty;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;  
            }
        }
        private  async Task<JsonEntityModel> GetBaseTypeEntity(JToken obj)
        {
            var model = new JsonEntityModel(obj);
            if (obj.HasValues)
            {
                foreach (var children in obj.Children().Values())
                {
                    if (children.Type == JTokenType.Array && children.HasValues) //数组处理
                    {
                        model.SubsetList.Add(new JsonEntityModel(children, await GetCollectionTypeEntity(children)));
                    }
                    else if (children.Count() > 1 && children.HasValues) //对象处理
                    {
                        model.SubsetList.Add(await GetBaseTypeEntity(children));
                    }
                    else //基类型处理
                    {
                        model.SubsetList.Add(await GetBaseTypeEntity(children));
                    }
                }
		model.VariableValue = $"[{model.SubsetList.Count}]";
            }
            else
            {
                model.IsArrayType = false;
                model.VariableValue = model.VariableType == (int)JTokenType.String ? $"\"{obj}\"" : $"{obj}";
                return model;
            }

            return model;
        }
        private  async Task<List<JsonEntityModel>> GetCollectionTypeEntity(JToken jToken)
        {
            var list = new List<JsonEntityModel>();
            for (int i = 0; i < jToken.Count(); i++)
            {
                list.Add(await GetBaseTypeEntity(jToken[i]));
            }
            return list;
        }

        private string ErrMsgHandler(string errMsg)
        {
            return string.Empty;
        }

        #endregion

        public void ScreenChanged(bool isFullScreen)
        {
            if(isFullScreen) GridColumnId = 1;
            else GridColumnId = 0;
        }

        public void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<object, string>(this, Const.CONST_TOOLMAIN_SCREENCHANGED, (obj, msg) => { ScreenChanged((bool)msg); });
        }
    }
}

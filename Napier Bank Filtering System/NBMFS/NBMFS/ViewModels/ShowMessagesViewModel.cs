using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NBMFS.Commands;
using System.Windows.Controls;
using NBMFS.Models;
using NBMFS.Database;
using System.Text.RegularExpressions;
using System.IO;
using NBMFS.Views;
using System.Data;
using Newtonsoft.Json;
using NBMFS.Database;
using static NBMFS.Database.SaveToFile;

namespace NBMFS.ViewModels
{
    //inherits from baseviewmodel
    //hold all the code that ShowMessagesViewModel will need to run
    public class ShowMessagesViewModel : BaseViewModel
    {
        //Buttons text
        public string ShowSmsButtonText { get; private set; }
        public string ShowTwitterButtonText { get; private set; }
        public string ShowEmailButtonText { get; private set; }
        public string ShowSirButtonText { get; private set; }
        //Button commands
        public ICommand CloseFormButtonCommand { get; private set; }
        public ICommand ShowSmsMessageButtonCommand { get; private set; }
        public ICommand ShowTwitterMessageButtonCommand { get; private set; }
        public ICommand ShowEmailMessageButtonCommand { get; private set; }
        public ICommand ShowSirMessageButtonCommand { get; private set; }
        //object list shown to bind to datagrid
        public ObservableCollection<object> MessageList { get; set; }

        public ShowMessagesViewModel()
        {
            ShowSmsButtonText = "Show Sms";
            ShowTwitterButtonText = "Show twitter";
            ShowEmailButtonText = "Show Email";
            ShowSirButtonText = "Show Sir";

            ShowSirMessageButtonCommand = new RelayCommand(ShowSirButtonClick);
            ShowEmailMessageButtonCommand = new RelayCommand(ShowEmailButtonClick);
            ShowSmsMessageButtonCommand = new RelayCommand(ShowSmsButtonClick);
            ShowTwitterMessageButtonCommand = new RelayCommand(ShowTwitterButtonClick);

            MessageList = new ObservableCollection<object>();
        }
        //add all desirialized SMS messages from json file to the list which will be shown on the data grid
        private void ShowSmsButtonClick()
        {
            MessageList.Clear();
            SaveToFile load = new SaveToFile();

            List<Sms> result = load.LoadJsonSms();

            foreach (var item in result)
            {
                MessageList.Add(item);
            }
        }
        //add all desirialized Tweet messages from json file to the list which will be shown on the data grid
        private void ShowTwitterButtonClick()
        {
            MessageList.Clear();
            SaveToFile file = new SaveToFile();

            var result = file.LoadJsonTweet();

            foreach (var item in result)
            {
                MessageList.Add(item);
            }
        }
        //add all desirialized Email messages from json file to the list which will be shown on the data grid
        private void ShowEmailButtonClick()
        {
            MessageList.Clear();
            SaveToFile file = new SaveToFile();

            var result = file.LoadJsonEmail();

            foreach (var item in result)
            {
                MessageList.Add(item);
            }
        }

        private void ShowSirButtonClick()
        {
            MessageList.Clear();
            SaveToFile file = new SaveToFile();

            var result = file.LoadJsonSir();

            foreach (var item in result)
            {
                MessageList.Add(item);
            }
        }
    }
}

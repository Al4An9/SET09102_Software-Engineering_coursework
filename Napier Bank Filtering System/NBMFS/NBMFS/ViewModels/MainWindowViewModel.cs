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
using Newtonsoft.Json;

namespace NBMFS.ViewModels
{
    //Inherits from BaseViewModel
    //Hold all the code that mainwindow will need to run 
    public class MainWindowViewModel : BaseViewModel
    {
        //there values are displayed on the UI
        //label text
        public string HeaderTextBlock { get; private set; }
        public string BodyTextBlock { get; private set; }
        //text boxes
        public string HeaderTextBox { get; set; }
        public string BodyTextBox { get; set; }
        //Button Commands
        public ICommand QuitButtonCommand { get; private set; }
        public ICommand ClearButtonCommand { get; private set; }
        public ICommand SendButtonCommand { get; private set; }
        public ICommand ShowMessageButtonCommand { get; private set; }
        //Button text
        public string ClearButtonText { get; private set; }
        public string SendButtonText { get; private set; }
        public string ShowMessageButtonText { get; private set; }
        public string QuitButtonText { get; private set; }

        public MainWindowViewModel()
        {
            HeaderTextBlock = "Message ID";
            BodyTextBlock = "Message Body";

            ClearButtonText = "Clear";
            SendButtonText = "Send";
            QuitButtonText = "Quit";
            ShowMessageButtonText = "Show Messages";

            //HeaderTextBox = string.Empty;
            HeaderTextBox = "123456789";
            BodyTextBox = string.Empty;


            QuitButtonCommand = new RelayCommand(QuitButtonClick);
            ClearButtonCommand = new RelayCommand(ClearButtonClick);
            SendButtonCommand = new RelayCommand(SendButtonClick);
            ShowMessageButtonCommand = new RelayCommand(ShowMessagesButtonClick);
        }

        //sends messages, checks validation and writes messages on a .csv file 
        private void SendButtonClick()
        {
            //lists that hold text found in the text
            List<string> hashtags = new List<string>();
            List<string> mentions = new List<string>();
            List<string> quarantined = new List<string>();
            List<string> sirs = new List<string>();
            Abbreviations abbreviations = new Abbreviations();

            //Check if textboxes are empty
            if (string.IsNullOrWhiteSpace(HeaderTextBox) || (string.IsNullOrWhiteSpace(BodyTextBox)))
            {
                MessageBox.Show("Please fill in the header and body textboxes appropriately", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Check if the message is an Sms with validations
            else if ((HeaderTextBox[0] == 'S') && (HeaderTextBox.Length == 10))
            {
                for (int id = 1; id < HeaderTextBox.Length; id++)
                {
                    if (!char.IsDigit(HeaderTextBox[id]))
                    {
                        MessageBox.Show("Message Id should contain only numbers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                int findspace = BodyTextBox.IndexOf(" ");
                string stopAt;
                //space found then write twitter sender id and continue with message
                if (findspace > 0)
                {
                    stopAt = BodyTextBox.Substring(0, findspace);

                    string sms_sender = stopAt;
                    //check for a valid mobile phone number
                    string phoneNumber = @"^(\+[0-9]{15})$";

                    if (!Regex.IsMatch(sms_sender, phoneNumber) && (sms_sender.Length < 15))
                    {
                        MessageBox.Show("SMS message body must begin with the senders international phone number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    //Assign sms sender and body to strings
                    string sms_body = BodyTextBox.Substring(16);
                    //check text length
                    if ((sms_body.Length <= 140) && (sms_body.Length > 0))
                    {
                        sms_body = abbreviations.ExpandSMS(sms_body);

                        // create new sms message
                        Sms message = new Sms()
                        {
                            Header = HeaderTextBox,
                            Sender = sms_sender,
                            Body = sms_body,
                            MType = "Sms"
                        };
                        //Save sms message to Json format 
                        SaveToFile save = new SaveToFile();
                        var smslist = save.LoadJsonSms();
                        smslist.Add(message);
                        string resultJson = JsonConvert.SerializeObject(smslist);
                        File.WriteAllText("sms.json", resultJson);

                        //check if json file exists                       
                        if (!File.Exists("sms.json"))
                        {
                            MessageBox.Show("Error while saving\n" + save.ErrorCode);
                        }
                        else
                        {
                            MessageBox.Show("Sms Message Send and saved", "Success", MessageBoxButton.OK);
                            save = null;
                        }
                        //prints message 
                        MessageBox.Show($"Message type: {message.MType}" +
                            $"\nMessageID: {message.Header}" +
                            $"\nSender: {message.Sender}" +
                            $"\nText: {message.Body}", "Your" + message.MType + "message have been send", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("Sms text must be up to 140 characters long or empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Sms body should start with a valid international phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            //check if it is a Twitter message with validations
            else if ((HeaderTextBox[0] == 'T') && (HeaderTextBox.Length == 10))
            {
                for (int id = 1; id < HeaderTextBox.Length; id++)
                {
                    //valid messageID?
                    if (!char.IsDigit(HeaderTextBox[id]))
                    {
                        MessageBox.Show("Message Id type letter must be followed by 9 numberic characters. Please try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                //checks if the sender twitterID is typed first in body
                if (BodyTextBox[0] == '@')
                {
                    int findspace = BodyTextBox.IndexOf(" ");
                    string stopAt;
                    //space found then write twitter sender id and continue with message
                    if (findspace > 0)
                    {
                        stopAt = BodyTextBox.Substring(0, findspace);

                        string tweet_sender_id = stopAt;
                        string tweetid = @"^@?(\w){1,15}$";

                        if (Regex.IsMatch(tweet_sender_id, tweetid))
                        {
                            int space = BodyTextBox.IndexOf(" ");

                            string tweet_body = BodyTextBox.Substring(space + 1);
                            if ((tweet_body.Length <= 140) && (tweet_body.Length > 0))
                            {
                                tweet_body = abbreviations.ExpandTweet(tweet_body);
                                //save any mentions found in tweet body to a list
                                foreach (Match match in Regex.Matches(input: tweet_body, pattern: @"(?<!\w)@\w+"))
                                {
                                    mentions.Add(match.Value);
                                    File.AppendAllText("mentions.csv", match.Value + Environment.NewLine);
                                }
                                //save any hashtags found in tweet body to a list
                                foreach (Match match in Regex.Matches(input: tweet_body, pattern: @"(?<!\w)#\w+"))
                                {
                                    hashtags.Add(match.Value);
                                    File.AppendAllText("hashtags.csv", match.Value + Environment.NewLine);
                                }

                                //add new twitter message
                                Tweet message = new Tweet()
                                {
                                    Header = HeaderTextBox,
                                    Sender = tweet_sender_id,
                                    Body = tweet_body,
                                    MType = "Tweet"
                                };
                                //Save file to Json format
                                SaveToFile save = new SaveToFile();
                                var tweetlist = save.LoadJsonTweet();
                                tweetlist.Add(message);
                                string resultJson = JsonConvert.SerializeObject(tweetlist);
                                File.WriteAllText("tweet.json", resultJson);

                                //check if json file exists
                                if (!File.Exists("tweet.json"))
                                {

                                    MessageBox.Show("Error while saving\n" + save.ErrorCode);
                                }
                                else
                                {
                                    MessageBox.Show("Tweet Message Send and saved", "Success", MessageBoxButton.OK);
                                    save = null;
                                }
                                //print message
                                MessageBox.Show($"Message type: {message.MType}" +
                                    $"\nMessageID: {message.Header}" +
                                    $"\nSender: {message.Sender}" +
                                    $"\nText: {message.Body}", "Your" + message.MType + "message have been send", MessageBoxButton.OK);

                                //print hashtag list and mention list
                                var hashtagslist = string.Join(Environment.NewLine, hashtags);
                                MessageBox.Show("Trending List:" + Environment.NewLine + hashtagslist, "Hashtag List", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                                var mentionslist = string.Join(Environment.NewLine, mentions);
                                MessageBox.Show("Mention List:" + Environment.NewLine + mentionslist, "Mention List", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            }
                            else
                            {
                                MessageBox.Show("Tweet text must be no longer than 140 characters and cant be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Tweet Body must begin with your Twitter ID. @ followed by maximum 15 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please allow a space after your TwitterID!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Tweet Body must begin with senders TwitterID. (ex. @AlexAn) ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            //check for email validations
            else if ((HeaderTextBox[0] == 'E') && (HeaderTextBox.Length == 10))
            {
                for (int id = 1; id < HeaderTextBox.Length; id++)
                {
                    //check if the 9 characters are digits
                    if (!char.IsDigit(HeaderTextBox[id]))
                    {
                        MessageBox.Show("Message Id should contain only numbers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                int findspace = BodyTextBox.IndexOf(" ");
                int findFullStop = BodyTextBox.IndexOf(". ");
                string stopAtFullStop;
                string stopAt;
                //space found then write email address and continue with subject
                if (findspace > 0)
                {
                    stopAt = BodyTextBox.Substring(0, findspace);
                    string sender_email = stopAt;

                    if (IsValidEmail(sender_email))
                    {
                        if (findFullStop > 0)
                        {
                            stopAtFullStop = BodyTextBox.Substring(findspace, findFullStop - findspace);
                            string email_subject = stopAtFullStop;
                            string SIRPattern = "^ +[S|s]+[I|i]+[R|r]+ \\d{2}/\\d{2}/\\d{2}$";

                            if ((Regex.IsMatch(email_subject, SIRPattern)) && (email_subject.Length <= 20))
                            {
                                string bodyStartAtFullStop = BodyTextBox.Substring(findFullStop + 1);
                                string email_body = bodyStartAtFullStop;

                                if ((email_body.Length <= 1029) && (email_body.Length > 0))
                                {

                                    foreach (Match match in Regex.Matches(input: email_body, pattern: @"\b\d\d-\d\d-\d\d\b|\bTheft\b"))
                                    {
                                        sirs.Add(match.Value);
                                        File.AppendAllText("sir.csv", match.Value + Environment.NewLine);
                                    }
                                    // add links found to a list
                                    foreach (Match match in Regex.Matches(input: email_body, pattern: @"\b(?:https?://|www\.)\S+\b"))
                                    {
                                        quarantined.Add(match.Value);
                                        File.AppendAllText("quarantined.csv", match.Value + Environment.NewLine);
                                    }
                                    //replace links found within email body
                                    email_body = Regex.Replace(email_body, @"\b(?:https?://|www\.)\S+\b", "<URL Quarantined>");

                                    //creates new email message
                                    SIR message = new SIR()
                                    {
                                        Header = HeaderTextBox,
                                        Sender = sender_email,
                                        Subject = email_subject,
                                        Body = email_body,
                                        MType = "SIR"
                                    };

                                    //Save email message to json
                                    SaveToFile save = new SaveToFile();
                                    var sirlist = save.LoadJsonSir();
                                    sirlist.Add(message);
                                    string resultJson = JsonConvert.SerializeObject(sirlist);
                                    File.WriteAllText("sir.json", resultJson);

                                    //check if file exists
                                    if (!File.Exists("sir.json"))
                                    {
                                        MessageBox.Show("Error while saving\n" + save.ErrorCode);
                                    }
                                    else
                                    {
                                        MessageBox.Show("SIR Message Send and saved", "Success", MessageBoxButton.OK);
                                        save = null;
                                    }
                                    //print message
                                    MessageBox.Show($"Message type: {message.MType}" +
                                        $"\nMessageID: {message.Header}" +
                                        $"\nSender: {message.Sender}" +
                                        $"\nMessage Subject:{message.Subject}" +
                                        $"\nText:{message.Body}", "Your" + message.MType + "message have been send", MessageBoxButton.OK);
                                    //print quarantined URLs
                                    var quarantinedlist = string.Join(Environment.NewLine, quarantined);
                                    MessageBox.Show("Quarantined urls List:" + Environment.NewLine + quarantinedlist, "Quarantined URL(s) added to the List", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                                    var sirs_list = string.Join(Environment.NewLine, sirs);
                                    MessageBox.Show("Sir List:" + Environment.NewLine + sirs_list, "Sir List", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                                }
                                else
                                {
                                    MessageBox.Show("SIR Body text must be no longer than 1028 characters!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else if (email_subject.Length <= 20)
                            {
                                string bodyStartAtFullStop = BodyTextBox.Substring(findFullStop + 1);
                                string email_body = bodyStartAtFullStop;
                                if (email_body.Length <= 1029)
                                {

                                    // add links found to a list
                                    foreach (Match match in Regex.Matches(input: email_body, pattern: @"\b(?:https?://|www\.)\S+\b"))
                                    {
                                        quarantined.Add(match.Value);
                                        File.AppendAllText("quarantined.csv", match.Value + Environment.NewLine);
                                    }
                                    //replace links found within email body
                                    email_body = Regex.Replace(email_body, @"\b(?:https?://|www\.)\S+\b", "<URL Quarantined>");
                                    //creates new email message
                                    Email message = new Email()
                                    {
                                        Header = HeaderTextBox,
                                        Sender = sender_email,
                                        Subject = email_subject,
                                        Body = email_body,
                                        MType = "Email"
                                    };

                                    //Save file to Json format
                                    SaveToFile save = new SaveToFile();
                                    var emaillist = save.LoadJsonEmail();
                                    emaillist.Add(message);
                                    string resultJson = JsonConvert.SerializeObject(emaillist);
                                    File.WriteAllText("email.json", resultJson);
                                    //check if file exists
                                    if (!File.Exists("email.json"))
                                    {
                                        MessageBox.Show("Error while saving\n" + save.ErrorCode);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Email Message Send and saved", "Success", MessageBoxButton.OK);
                                        save = null;
                                    }
                                    //print message
                                    MessageBox.Show($"Message type: {message.MType}" +
                                        $"\nMessageID: {message.Header}" +
                                        $"\nSender: {message.Sender}" +
                                        $"\nMessage Subject:{message.Subject}" +
                                        $"\nText:{message.Body}", "Your" + message.MType + "message have been send", MessageBoxButton.OK);
                                    //print quarantined URLs
                                    var quarantinedlist = string.Join(Environment.NewLine, quarantined);
                                    MessageBox.Show("Quarantined urls List:" + Environment.NewLine + quarantinedlist, "Quarantined URLs List", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                                }
                                else
                                {
                                    MessageBox.Show("Email Body text must be no longer than 1028 characters!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Email Subject must be no longer than 20 characters!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please mind that subject should be less than 20 characters long, and remember to seperate subject and message with a full stop and a space!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email Body must begin with a valid Email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Email Body must begin with a valid Email address followed by a space.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("MessageID is not valid. Please enter a valid messageID maximum 10 characters long.\n" +
                                "First character will indicate the message type and the rest must be numeric characters.\n" +
                                "ex. S123456789 for SMS, T876543210 for Tweets, E147258360 for Emails!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        //mathod called in the email section to check for a valid email address
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        //clears the textboxes
        private void ClearButtonClick()
        {
            HeaderTextBox = string.Empty;
            BodyTextBox = string.Empty;

            //calling OnChanged method to letting it know that there ha been a change in a property
            OnChanged(nameof(HeaderTextBox));
            OnChanged(nameof(BodyTextBox));
        }
        //opens the another window where you can view messages
        private void ShowMessagesButtonClick()
        {
            ShowAllMessagesViewModels objShowAllMessages = new ShowAllMessagesViewModels();
            objShowAllMessages.Show();
        }
        // closes the application
        private void QuitButtonClick()
        {
            Application.Current.Shutdown();
        }
    }
}
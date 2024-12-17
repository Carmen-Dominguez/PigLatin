/*Author:   Carmen Dominguez
 Created:   11 December 2017*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace PigLatin
{
    class Program
    {
        //Variables
        static int x = 1;
        static IDictionary<int, string> dict = new Dictionary<int, string>();
        static Database db = new Database();

        //The Bot
        private static TelegramBotClient Bot = new TelegramBotClient("474984703:AAFXDgNWP7X89YPF9JWb3aHJTjgZ_wmI8pA");

        public void InitPic()
        {
            dict.Add(1, "Hello Sky");
            dict.Add(2, "Hello Island");
            dict.Add(3, "Hello Sea");
            dict.Add(4, "Hello Ocean");
            dict.Add(5, "Hello World");
            dict.Add(6, "Hello Pale Blue Dot");
            dict.Add(7, "Hello Galaxy");
            dict.Add(8, "Hello Galaxy Cluster");
            dict.Add(9, "Hello Universe");
            dict.Add(10, "Hello Multi Universe");
            dict.Add(11, "Ran Out of Existance");
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.TextMessage) return;

            //If message contains something
            db.ConnectDB();
            String chatID = message.Chat.Id.ToString();
            String name = message.Chat.FirstName + " " + message.Chat.LastName; /*Get the username from Telegram*/
            String pass = "";

            if (message.Text.StartsWith("/start"))
            {
                if (db.AddTelUser(chatID, name, pass))
                {
                    Console.WriteLine(chatID + " has been added");
                    await Bot.SendTextMessageAsync(message.Chat.Id, "You have been added to SimpleBot.");
                }
                else
                {
                    Console.WriteLine(chatID + " couldn't be added");
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Something went wrong. You may already be subscribed.");
                }
            }
            else

            if (message.Text.StartsWith("/text"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Message was received");
            }
            else

                if (message.Text.StartsWith("/photo zoom out"))
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
                string b = dict[x++];
                string file = @"C:\\Users\cdomi\source\repos\PigLatin\PigLatin\img\" + b + ".jpg";

                var fileName = file.Split('\\').Last();

                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var fts = new FileToSend(fileName, fileStream);

                    await Bot.SendPhotoAsync(message.Chat.Id, fts, b);
                }
            }

            else
                //Add user to task and/or return task details
                foreach (string taskID in db.GetTaskIDs())
                {
                    if (message.Text.StartsWith("/" + taskID))
                    {
                        if (db.CheckPass(name, pass)) /*Check password*/
                        {
                            if (db.AddToTask(taskID, chatID))
                            {
                                Console.WriteLine(chatID + " has been added to " + taskID);
                                await Bot.SendTextMessageAsync(message.Chat.Id, "You have been added to The " + taskID + " Group.");
                            }
                            await Bot.SendTextMessageAsync(message.Chat.Id, db.GetTask(taskID).GetString(0));
                        }
                    }
                }
        }

        //send Tasks to members
        public void Task(string mess)
        {
            foreach (string taskID in db.GetTaskIDs())
            {
                if (mess == taskID)
                {
                    string task = db.GetTask(taskID).GetString(0);
                    foreach (string chatID in db.GetMemberIDs(taskID))
                    {
                        Bot.SendTextMessageAsync(chatID, task);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Program bla = new Program();
            bla.InitPic();
            Bot.OnMessage += BotOnMessageReceived;

            ChatId carmen = "177678603";

            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;
            Bot.StartReceiving();
            while (true)
            {
                String mess = "" + Console.ReadLine();
                bla.Task(mess);
                Bot.SendTextMessageAsync(carmen, mess);
            }
        }
    }
}


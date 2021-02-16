using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        List<string> notificationList = new List<string>();

        public void ShowNews()
        {
            foreach (string news in GetAllNotifications())
            {
                if (UIManager.Instance.newsGO.activeSelf == false)
                {
                    UIManager.Instance.News(news);
                    GetAllNotifications().Remove(news);
                    break;
                }
            }
        }

        public void CreateNotification(string message)
        {
            notificationList.Add(message);
        }

        public List<string> GetAllNotifications()
        {
            return notificationList;
        }
    }
}
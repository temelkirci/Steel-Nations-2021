using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        List<string> publicNotificationList = new List<string>();
        List<string> privateNotificationList = new List<string>();

        public void ShowNews()
        {
            foreach (string news in GetPublicNotifications())
            {
                if (UIManager.Instance.newsGO.activeSelf == false)
                {
                    UIManager.Instance.PublicNotification(news);
                    GetPublicNotifications().Remove(news);
                    break;
                }
            }

            foreach (string news in GetPrivateNotifications())
            {
                if (UIManager.Instance.newsGO.activeSelf == false)
                {
                    UIManager.Instance.PublicNotification(news);
                    GetPublicNotifications().Remove(news);
                    break;
                }
            }
        }

        public void CreatePublicNotification(string message)
        {
            publicNotificationList.Add(message);
        }

        public List<string> GetPublicNotifications()
        {
            return publicNotificationList;
        }

        public void CreatePrivateNotification(string message)
        {
            privateNotificationList.Add(message);
        }

        public List<string> GetPrivateNotifications()
        {
            return privateNotificationList;
        }
    }
}
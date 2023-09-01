using System;
using System.Collections.Generic;
using EvoS.Framework.DataAccess;
using EvoS.Framework.DataAccess.Daos;

namespace CentralServer.LobbyServer.Chat;

public static class AdminMessageManager
{
    public static string PopAdminMessage(long accountId)
    {
        AdminMessageDao dao = DB.Get().AdminMessageDao;
        AdminMessageDao.AdminMessage msg = dao.FindPending(accountId);
        if (msg is null)
        {
            return null;
        }

        msg.viewed = true;
        msg.viewedAt = DateTime.UtcNow;
        dao.Save(msg);
        
        return msg.message;
    }

    public static void SendAdminMessage(long accountId, long adminAccountId, string message)
    {
        AdminMessageDao.AdminMessage msg = new AdminMessageDao.AdminMessage
        {
            accountId = accountId,
            adminAccountId = adminAccountId,
            message = message,
            createdAt = DateTime.UtcNow
        };
        DB.Get().AdminMessageDao.Save(msg);
    }

    public static List<AdminMessageDao.AdminMessage> GetAdminMessages(long accountId)
    {
        return DB.Get().AdminMessageDao.Find(accountId);
    }
}
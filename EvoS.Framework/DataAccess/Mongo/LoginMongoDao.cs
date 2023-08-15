using System.Collections.Generic;
using System.Text.RegularExpressions;
using EvoS.Framework.Auth;
using EvoS.Framework.DataAccess.Daos;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class LoginMongoDao : MongoDao<long, LoginDao.LoginEntry>, LoginDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AccountMongoDao));
        
        public LoginMongoDao() : base("logins")
        {
        }

        public LoginDao.LoginEntry Find(string username)
        {
            return c.Find(f.Eq("Username", username)).FirstOrDefault();
        }

        public List<LoginDao.LoginEntry> FindRegex(string username)
        {
            return c.Find(f.Regex("Username", new BsonRegularExpression(Regex.Escape(username), "i"))).ToList();
        }

        public List<LoginDao.LoginEntry> FindAll()
        {
            return c.Find(f.Empty).ToList();
        }

        public LoginDao.LoginEntry Find(long accountId)
        {
            return findById(accountId);
        }

        // TODO index
        public LoginDao.LoginEntry FindByLinkedAccount(LinkedAccount linkedAccount)
        {
            return c.Find(f.ElemMatch(
                "LinkedAccounts",
                f.And(
                    f.Eq("type", linkedAccount.Type),
                    f.Eq("id", linkedAccount.Id))
                )).FirstOrDefault();
        }

        public void Save(LoginDao.LoginEntry entry)
        {
            insert(entry.AccountId, entry);
        }
    }
}
using System;
using System.Collections.Generic;
using EvoS.Framework.DataAccess.Daos;

namespace EvoS.Framework.DataAccess.Mock
{
    public class RegistrationCodeMockDao: RegistrationCodeDao
    {
        public RegistrationCodeDao.RegistrationCodeEntry Find(string code)
        {
            return null;
        }

        public List<RegistrationCodeDao.RegistrationCodeEntry> FindBefore(int limit, DateTime dateTime)
        {
            return new List<RegistrationCodeDao.RegistrationCodeEntry>();
        }

        public List<RegistrationCodeDao.RegistrationCodeEntry> FindAll(int limit, int offset)
        {
            return new List<RegistrationCodeDao.RegistrationCodeEntry>();
        }

        public void Save(RegistrationCodeDao.RegistrationCodeEntry entry)
        {
        }
    }
}
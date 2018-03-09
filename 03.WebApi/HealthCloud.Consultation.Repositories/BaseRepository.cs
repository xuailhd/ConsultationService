using HealthCloud.Consultation.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;

namespace HealthCloud.Consultation.Repositories
{
    public class RepositoryProxy<T> : RealProxy
    {
        private T _target;
        public RepositoryProxy(T target) : base(typeof(T))
        {
            this._target = target;
        }
        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage callMessage = (IMethodCallMessage)msg;
            object[] args = callMessage.Args;
            var dbPrivateFlag = false;
            DBEntities db = null;
            int i = 0;
            List<int> outParaIndexs = new List<int>();
            foreach (var para in callMessage.MethodBase.GetParameters())
            {
                if (para.ParameterType.FullName == typeof(DBEntities).FullName)
                {
                    if (args[i] == null)
                    {
                        db = new DBEntities();
                        args[i] = db;
                        dbPrivateFlag = true;
                    }
                }
                i++;
            }
            try
            {
                object returnValue = callMessage.MethodBase.Invoke(this._target, args);
                return new ReturnMessage(returnValue, args, args.Length, null, callMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbPrivateFlag && db != null)
                {
                    db.Dispose();
                }
            }
        }
    }

    public static class TransparentProxy
    {
        public static T Create<T>()
        {
            T instance = Activator.CreateInstance<T>();
            RepositoryProxy<T> realProxy = new RepositoryProxy<T>(instance);
            T transparentProxy = (T)realProxy.GetTransparentProxy();
            return transparentProxy;
        }
    }

    public class BaseRepository: MarshalByRefObject
    {
        public DBEntities CreateDb()
        {
            return new DBEntities();
        }

        public bool SubmitChange(DBEntities db, bool disposable = true)
        {
            var ret = false;
            try
            {
                ret = db.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                if (disposable)
                {
                    db.Dispose();
                }
                throw ex;
            }

            if (disposable)
            {
                db.Dispose();
            }
            return ret;
        }

        public void Dispose(DBEntities db)
        {
            db.Dispose();
        }
    }
}

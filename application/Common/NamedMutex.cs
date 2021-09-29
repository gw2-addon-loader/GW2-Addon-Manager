using System;
using System.Threading;                 //Mutex
using System.Security.AccessControl;    //MutexAccessRule
using System.Security.Principal;        //SecurityIdentifier

namespace GW2AddonManager
{
    // Largely taken from https://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c
    class NamedMutex : IDisposable
    {
        Mutex _mutex;
        bool _acquired;

        public NamedMutex(string name, bool global)
        {
            // unique id for global mutex - Global prefix means it is global to the machine
            string mutexId = global ? $"Global\\{name}" : name;

            // Need a place to store a return value in Mutex() constructor call
            bool createdNew;

            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            var allowEveryoneRule =
                new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                    MutexRights.FullControl,
                                    AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
            _mutex = new Mutex(false, mutexId, out createdNew);
            _mutex.SetAccessControl(securitySettings);

            // edited by acidzombie24
            _acquired = false;
            try {
                // note, you may want to time out here instead of waiting forever
                // edited by acidzombie24
                _acquired = _mutex.WaitOne(5000, false);
                if (_acquired == false)
                    throw new TimeoutException("Timeout waiting for exclusive access");
            }
            catch (AbandonedMutexException) {
                // Log the fact that the mutex was abandoned in another process,
                // it will still get acquired
                _acquired = true;
            }
        }

        public void Dispose()
        {
            if (_acquired)
                _mutex.ReleaseMutex();
            _acquired = false;
        }
    }
}

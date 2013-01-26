using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windows_qPaste
{
    public class Token
    {
        public string token
        {
            get;
            set;
        }

        public string link
        {
            get;
            set;
        }

        public Storage storage
        {
            get;
            set;
        }

        public class Storage
        {
            public string s3PolicyBase64
            {
                get;
                set;
            }

            public string s3Signature
            {
                get;
                set;
            }

            public string s3Key
            {
                get;
                set;
            }

            public Policy s3Policy
            {
                get;
                set;
            }
        }

        public class Policy
        {
            public string expiration
            {
                get;
                set;
            }

            public Conditions conditions
            {
                get;
                set;
            }
        }

        public class Conditions
        {
            public string key
            {
                get;
                set;
            }
            public string bucket
            {
                get;
                set;
            }

            public string acl
            {
                get;
                set;
            }

            public string mime
            {
                get;
                set;
            }
        }
    }
}

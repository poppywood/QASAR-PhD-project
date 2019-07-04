using System;
using System.Collections.Generic;
using System.Text;

namespace Qasar.ESB.Filter
{
    /// <summary>
    /// Holds the data that's passed through the pipline filters
    /// </summary>
    public class PipeData
    {
        readonly string _requestType;
        readonly string _subAction;
        readonly string _productCode;
        readonly string _source;
        readonly string _userId;
        readonly string _reference;
        readonly IEntity _entity;

        ConfigEngine.Rule _rule;
        string _request;
        string _snapshot;
        int _resourceID;
        int _classID;

        bool _success = true;
        bool _notification = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="subAction"></param>
        /// <param name="productCode"></param>
        /// <param name="source"></param>
        /// <param name="userId"></param>
        /// <param name="reference"></param>
        /// <param name="request"></param>
        public PipeData(
                string action,
                string subAction,
                string productCode,
                string source,
                string userId,
                string reference,
                string request,
                int resourceID,
                IEntity entity
            )
        {
            _requestType  = action;
            _subAction  = subAction;
            _productCode  = productCode;
            _source  = source;
            _userId  = userId;
            _reference  = reference;
            _request = request;
            _resourceID = resourceID;
            _entity = entity;
        }

        /// <summary>
        /// sets _success = false. do it this was rather than settable property
        /// because _success should never be set back to true.
        /// </summary>
        public void SetFailure()
        {
            _success = false;
        }

        /// <summary>
        /// sets _notification = true. do it this way so that it can't be set back to false.
        /// </summary>
        public void SetNotificationFailure()
        {
            _notification = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public string RequestType
        {
            get { return _requestType; }
        }

        /// <summary>
        /// SubAction
        /// </summary>
        public string SubAction
        {
            get { return _subAction; }
        }

        /// <summary>
        /// ProductCode
        /// </summary>
        public string ProductCode
        {
            get { return _productCode; }
        }

        /// <summary>
        /// Source
        /// </summary>
        public string Source
        {
            get { return _source; }
        }

        /// <summary>
        /// UserId
        /// </summary>
        public string UserId
        {
            get { return _userId; }
        }

        /// <summary>
        /// Reference
        /// </summary>
        public string Reference
        {
            get { return _reference; }
        }

        /// <summary>
        /// Entity
        /// </summary>
        public IEntity Entity
        {
            get { return _entity; }
        }

        /// <summary>
        /// Rule
        /// </summary>
        public ConfigEngine.Rule Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }

        /// <summary>
        /// Request
        /// </summary>
        public string Request
        {
            get { return _request; }
            set { _request = value; }
        }

        /// <summary>
        /// Snapshot
        /// </summary>
        public string Snapshot
        {
            get { return _snapshot; }
            set { _snapshot = value; }
        }

        /// <summary>
        /// ResourceID
        /// </summary>
        public int ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }
        }

        /// <summary>
        /// ClassID
        /// </summary>
        public int ClassID
        {
            get { return _classID; }
            set { _classID = value; }
        }

        /// <summary>
        /// Success
        /// </summary>
        public bool Success
        {
            get { return _success; }
        }

        /// <summary>
        /// Failed
        /// </summary>
        public bool Failed
        {
            get { return !_success; }
        }

        /// <summary>
        /// Notification
        /// </summary>
        public bool Notification
        {
            get { return _notification; }
        }

    }
}

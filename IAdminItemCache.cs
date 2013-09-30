//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace CarDepot
{
    public interface IAdminItemCache
    {
        event EventHandler<AdminItemCache.RefreshEventArgs> RefreshStarted;
        event EventHandler<AdminItemCache.RefreshEventArgs> RefreshEnded;
        event EventHandler<AdminItemCache.UpdateEventArgs> ItemUpdate;


        void AddItem(IAdminObject item);
        bool ContainsKey(string objectId);
        int Count { get; }
        void ExitReadLock();
        bool InitialRefreshPerformed { get; }
        void ModifyItem(IAdminObject item);
        void Refresh();
        IAdminObject RefreshItem(IAdminObject item);
        void RemoveItem(string objectId);
        IAdminObject this[string objectId] { get; set; }
        List<T> ToList<T>() where T : IAdminObject;
        bool TryEnterReadLock();
    }

}

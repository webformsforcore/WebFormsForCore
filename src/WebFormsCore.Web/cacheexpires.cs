#line 1 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"






namespace System.Web.Caching {
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Util;
    using System.Collections;

    
    
    
    
    
    
    
    
    struct ExpiresEntryRef {
        
        static internal readonly ExpiresEntryRef INVALID = new ExpiresEntryRef(0, 0);

        const uint   ENTRY_MASK  = 0x000000ffu;
        const uint   PAGE_MASK   = 0xffffff00u;
        const int    PAGE_SHIFT  = 8;

        uint _ref;

        internal ExpiresEntryRef(int pageIndex, int entryIndex) {
            Debug.Assert((pageIndex & 0x00ffffff) == pageIndex, "(pageIndex & 0x00ffffff) == pageIndex");
            Debug.Assert((entryIndex & ENTRY_MASK) == entryIndex, "(entryIndex & ENTRY_MASK) == entryIndex");
            Debug.Assert(entryIndex != 0 || pageIndex == 0, "entryIndex != 0 || pageIndex == 0");

            _ref = ( (((uint)pageIndex) << PAGE_SHIFT) | (((uint)(entryIndex)) & ENTRY_MASK) );
        }

        public override bool Equals(object value) {
            if (value is ExpiresEntryRef) {
                return _ref == ((ExpiresEntryRef)value)._ref;
            }

            return false;
        }




#line 53 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"
        public static bool operator !=(ExpiresEntryRef r1, ExpiresEntryRef r2) {
            return r1._ref != r2._ref;
        }
        public static bool operator ==(ExpiresEntryRef r1, ExpiresEntryRef r2) {
            return r1._ref == r2._ref;
        }
        
        public override int GetHashCode() {
            return (int) _ref;
        }
    




#line 69 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"

        
        internal int PageIndex {
            get {
                int result = (int) (_ref >> PAGE_SHIFT);
                return result;
            }
        }

        
        
        internal int Index {
            get {
                int result = (int) (_ref & ENTRY_MASK);
                return result;
            }
        }

        
        internal bool IsInvalid {
            get {
                return _ref == 0;
            }
        }
    }

    
    [StructLayout(LayoutKind.Explicit)]
    struct ExpiresEntry {
        
        
        
        
        
        [FieldOffset(0)]
        internal DateTime           _utcExpires;    

        [FieldOffset(0)]
        internal ExpiresEntryRef    _next;          

        [FieldOffset(4)]
        internal int                _cFree;         
                                                    
        [FieldOffset(8)]
        internal CacheEntry         _cacheEntry;    
    }

    
    struct ExpiresPage {
        internal ExpiresEntry[] _entries;       
        internal int            _pageNext;      
        internal int            _pagePrev;      
    }

    
    struct ExpiresPageList {
        internal int            _head;          
        internal int            _tail;          
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    sealed class ExpiresBucket {
        
        internal const int  NUM_ENTRIES     = 127;
        const int           LENGTH_ENTRIES  = 128;

        const int           MIN_PAGES_INCREMENT = 10;   
        const int           MAX_PAGES_INCREMENT = 340;   
        const double        MIN_LOAD_FACTOR = 0.5;      

        const int                   COUNTS_LENGTH=4;    

        
        static readonly TimeSpan    COUNT_INTERVAL= new TimeSpan(CacheExpires._tsPerBucket.Ticks / COUNTS_LENGTH);

        readonly CacheExpires   _cacheExpires;          
        readonly byte           _bucket;                

        ExpiresPage[]           _pages;                 



#line 195 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"
        int                     _cEntriesInUse;         


#line 199 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"
        int                     _cPagesInUse;           


#line 203 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"
        int                     _cEntriesInFlush;       


#line 207 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"
        int                     _minEntriesInUse;       

        ExpiresPageList         _freePageList;          
        ExpiresPageList         _freeEntryList;         



#line 215 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"
        bool                    _blockReduce;           

        
        
        
        
        DateTime                _utcMinExpires;         

        
        
        
        int[]                   _counts;

        
        DateTime                _utcLastCountReset;

        internal ExpiresBucket(CacheExpires cacheExpires, byte bucket, DateTime utcNow) {
            _cacheExpires = cacheExpires;
            _bucket = bucket;
            _counts = new int[COUNTS_LENGTH];
            ResetCounts(utcNow);
            InitZeroPages();

            Debug.Validate("CacheValidateExpires", this);
        }

        void InitZeroPages() {
            Debug.Assert(_cPagesInUse == 0, "_cPagesInUse == 0");
            Debug.Assert(_cEntriesInUse == 0, "_cEntriesInUse == 0");
            Debug.Assert(_cEntriesInFlush == 0, "_cEntriesInFlush == 0");

            _pages = null;
            _minEntriesInUse = -1;
            _freePageList._head = -1;
            _freePageList._tail = -1;
            _freeEntryList._head = -1;
            _freeEntryList._tail = -1;
        }


















#line 272 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"

        
        void ResetCounts(DateTime utcNow) {
            _utcLastCountReset = utcNow;
            _utcMinExpires = DateTime.MaxValue;

            for (int i = 0; i < _counts.Length; i++) {
                _counts[i] = 0;
            }
        }

        
        
        
        int GetCountIndex(DateTime utcExpires) {
            return Math.Max(0, (int) ((utcExpires - _utcLastCountReset).Ticks / COUNT_INTERVAL.Ticks));
        }

        
        void AddCount(DateTime utcExpires) {
            int ci = GetCountIndex(utcExpires);
            for (int i = _counts.Length - 1; i >= ci; i--) {
                _counts[i]++;
            }

            if (utcExpires < _utcMinExpires) {
                _utcMinExpires = utcExpires;
            }
        }

        
        void RemoveCount(DateTime utcExpires) {
            int ci = GetCountIndex(utcExpires);
            for (int i = _counts.Length - 1; i >= ci; i--) {
                _counts[i]--;
            }
        }

        
        int GetExpiresCount(DateTime utcExpires) {
            if (utcExpires < _utcMinExpires)
                return 0;

            int ci = GetCountIndex(utcExpires);
            if (ci >= _counts.Length)
                return _cEntriesInUse;

            return _counts[ci];
        }

        
        void AddToListHead(int pageIndex, ref ExpiresPageList list) {
            Debug.Assert((list._head == -1) == (list._tail == -1), "(list._head == -1) == (list._tail == -1)");

            (_pages[(pageIndex)]._pagePrev) = -1;
            (_pages[(pageIndex)]._pageNext) = list._head;
            if (list._head != -1) {
                Debug.Assert((_pages[(list._head)]._pagePrev) == -1, "PagePrev(list._head) == -1");
                (_pages[(list._head)]._pagePrev) = pageIndex;
            }
            else {
                list._tail = pageIndex;
            }

            list._head = pageIndex;
        }

        
        void AddToListTail(int pageIndex, ref ExpiresPageList list) {
            Debug.Assert((list._head == -1) == (list._tail == -1), "(list._head == -1) == (list._tail == -1)");

            (_pages[(pageIndex)]._pageNext) = -1;
            (_pages[(pageIndex)]._pagePrev) = list._tail;
            if (list._tail != -1) {
                Debug.Assert((_pages[(list._tail)]._pageNext) == -1, "PageNext(list._tail) == -1");
                (_pages[(list._tail)]._pageNext) = pageIndex;
            }
            else {
                list._head = pageIndex;
            }

            list._tail = pageIndex;
        }

        
        int RemoveFromListHead(ref ExpiresPageList list) {
            Debug.Assert(list._head != -1, "list._head != -1");

            int oldHead = list._head;
            RemoveFromList(oldHead, ref list);
            return oldHead;
        }

        
        void RemoveFromList(int pageIndex, ref ExpiresPageList list) {
            Debug.Assert((list._head == -1) == (list._tail == -1), "(list._head == -1) == (list._tail == -1)");

            if ((_pages[(pageIndex)]._pagePrev) != -1) {
                Debug.Assert((_pages[((_pages[(pageIndex)]._pagePrev))]._pageNext) == pageIndex, "PageNext(PagePrev(pageIndex)) == pageIndex");
                (_pages[((_pages[(pageIndex)]._pagePrev))]._pageNext) = (_pages[(pageIndex)]._pageNext);
            }
            else {
                Debug.Assert(list._head == pageIndex, "list._head == pageIndex");
                list._head = (_pages[(pageIndex)]._pageNext);
            }

            if ((_pages[(pageIndex)]._pageNext) != -1) {
                Debug.Assert((_pages[((_pages[(pageIndex)]._pageNext))]._pagePrev) == pageIndex, "PagePrev(PageNext(pageIndex)) == pageIndex");
                (_pages[((_pages[(pageIndex)]._pageNext))]._pagePrev) = (_pages[(pageIndex)]._pagePrev);
            }
            else {
                Debug.Assert(list._tail == pageIndex, "list._tail == pageIndex");
                list._tail = (_pages[(pageIndex)]._pagePrev);
            }

            (_pages[(pageIndex)]._pagePrev) = -1;
            (_pages[(pageIndex)]._pageNext) = -1;
        }

        
        void MoveToListHead(int pageIndex, ref ExpiresPageList list) {
            Debug.Assert(list._head != -1, "list._head != -1");
            Debug.Assert(list._tail != -1, "list._tail != -1");

            
            if (list._head == pageIndex)
                return;

            
            RemoveFromList(pageIndex, ref list);

            
            AddToListHead(pageIndex, ref list);
        }

        
        void MoveToListTail(int pageIndex, ref ExpiresPageList list) {
            Debug.Assert(list._head != -1, "list._head != -1");
            Debug.Assert(list._tail != -1, "list._tail != -1");

            
            if (list._tail == pageIndex)
                return;

            
            RemoveFromList(pageIndex, ref list);

            
            AddToListTail(pageIndex, ref list);
        }

        
        
        
        
        void UpdateMinEntries() {
            if (_cPagesInUse <= 1) {
                _minEntriesInUse = -1;
            }
            else {
                int capacity = _cPagesInUse * NUM_ENTRIES;
                Debug.Assert(capacity > 0, "capacity > 0");
                Debug.Assert(MIN_LOAD_FACTOR < 1.0, "MIN_LOAD_FACTOR < 1.0");

                _minEntriesInUse = (int) (capacity * MIN_LOAD_FACTOR);

                
                
                if ((_minEntriesInUse - 1) > ((_cPagesInUse - 1) * NUM_ENTRIES)) {
                    _minEntriesInUse = -1;
                }
            }





#line 450 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"

        }

        
        void RemovePage(int pageIndex) {
            Debug.Assert((((_pages[(pageIndex)]._entries))[0]._cFree) == NUM_ENTRIES, "FreeEntryCount(EntriesI(pageIndex)) == NUM_ENTRIES");

            
            RemoveFromList(pageIndex, ref _freeEntryList);

            
            AddToListHead(pageIndex, ref _freePageList);

            
            Debug.Assert((_pages[(pageIndex)]._entries) != null, "EntriesI(pageIndex) != null");
            (_pages[(pageIndex)]._entries) = null;

            
            _cPagesInUse--;
            if (_cPagesInUse == 0) {
                InitZeroPages();
            }
            else {
                UpdateMinEntries();
            }
        }

        
        ExpiresEntryRef GetFreeExpiresEntry() {
            
            Debug.Assert(_freeEntryList._head >= 0, "_freeEntryList._head >= 0");
            int pageIndex = _freeEntryList._head;

            
            ExpiresEntry[] entries = (_pages[(pageIndex)]._entries);
            int entryIndex = ((entries)[0]._next).Index;

            
            ((entries)[0]._next) = entries[entryIndex]._next;
            ((entries)[0]._cFree)--;
            if (((entries)[0]._cFree) == 0) {
                
                Debug.Assert(((entries)[0]._next).IsInvalid, "FreeEntryHead(entries).IsInvalid");
                RemoveFromList(pageIndex, ref _freeEntryList);
            }







#line 503 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"

            return new ExpiresEntryRef(pageIndex, entryIndex);
        }


        
        void AddExpiresEntryToFreeList(ExpiresEntryRef entryRef) {
            ExpiresEntry[] entries = (_pages[(entryRef.PageIndex)]._entries);
            int entryIndex = entryRef.Index;

            Debug.Assert(entries[entryIndex]._cacheEntry == null, "entries[entryIndex]._cacheEntry == null");
            entries[entryIndex]._cFree = 0;

            entries[entryIndex]._next = ((entries)[0]._next);
            ((entries)[0]._next) = entryRef;

            _cEntriesInUse--;
            int pageIndex = entryRef.PageIndex;
            ((entries)[0]._cFree)++;
            if (((entries)[0]._cFree) == 1) {
                
                AddToListHead(pageIndex, ref _freeEntryList);
            }
            else if (((entries)[0]._cFree) == NUM_ENTRIES) {
                RemovePage(pageIndex);
            }
        }

        
        
        
        void Expand() {
            Debug.Assert(_cPagesInUse * NUM_ENTRIES == _cEntriesInUse, "_cPagesInUse * NUM_ENTRIES == _cEntriesInUse");
            Debug.Assert(_freeEntryList._head == -1, "_freeEntryList._head == -1");
            Debug.Assert(_freeEntryList._tail == -1, "_freeEntryList._tail == -1");

            
            if (_freePageList._head == -1) {
                
                int oldLength;
                if (_pages == null) {
                    oldLength = 0;
                }
                else {
                    oldLength = _pages.Length;
                }

                Debug.Assert(_cPagesInUse == oldLength, "_cPagesInUse == oldLength");
                Debug.Assert(_cEntriesInUse == oldLength * NUM_ENTRIES, "_cEntriesInUse == oldLength * ExpiresEntryRef.NUM_ENTRIES");

                int newLength = oldLength * 2;
                newLength = Math.Max(oldLength + MIN_PAGES_INCREMENT, newLength);
                newLength = Math.Min(newLength, oldLength + MAX_PAGES_INCREMENT);
                Debug.Assert(newLength > oldLength, "newLength > oldLength");

                ExpiresPage[] newPages = new ExpiresPage[newLength];

                
                for (int i = 0; i < oldLength; i++) {
                    newPages[i] = _pages[i];
                }

                
                for (int i = oldLength; i < newPages.Length; i++) {
                    newPages[i]._pagePrev = i - 1;
                    newPages[i]._pageNext = i + 1;
                }

                newPages[oldLength]._pagePrev = -1;
                newPages[newPages.Length - 1]._pageNext = -1;

                
                _freePageList._head = oldLength;
                _freePageList._tail = newPages.Length - 1;

                _pages = newPages;
            }

            
            int pageIndex = RemoveFromListHead(ref _freePageList);
            AddToListHead(pageIndex, ref _freeEntryList);

            
            ExpiresEntry[] entries = new ExpiresEntry[LENGTH_ENTRIES];
            ((entries)[0]._cFree) = NUM_ENTRIES;
            
            
            for (int i = 0; i < entries.Length - 1; i++) {
                entries[i]._next = new ExpiresEntryRef(pageIndex, i + 1);
            }
            entries[entries.Length - 1]._next = ExpiresEntryRef.INVALID;

            (_pages[(pageIndex)]._entries) = entries;

            
            _cPagesInUse++;
            UpdateMinEntries();
        }

        
        
        void Reduce() {
            
            if (_cEntriesInUse >= _minEntriesInUse || _blockReduce)
                return;

            Debug.Assert(_freeEntryList._head != -1, "_freeEntryList._head != -1");
            Debug.Assert(_freeEntryList._tail != -1, "_freeEntryList._tail != -1");
            Debug.Assert(_freeEntryList._head != _freeEntryList._tail, "_freeEntryList._head != _freeEntryList._tail");

            
            int meanFree = (int) (NUM_ENTRIES - (NUM_ENTRIES * MIN_LOAD_FACTOR));
            int pageIndexLast = _freeEntryList._tail;
            int pageIndexCurrent = _freeEntryList._head;
            int pageIndexNext;
            ExpiresEntry[] entries;

            for (;;) {
                pageIndexNext = (_pages[(pageIndexCurrent)]._pageNext);

                
                
                if ((((_pages[(pageIndexCurrent)]._entries))[0]._cFree) > meanFree) {
                    MoveToListTail(pageIndexCurrent, ref _freeEntryList);
                }
                else {
                    MoveToListHead(pageIndexCurrent, ref _freeEntryList);
                }

                
                if (pageIndexCurrent == pageIndexLast)
                    break;

                
                pageIndexCurrent = pageIndexNext;
            }

            
            
            for (;;) {
                
                if (_freeEntryList._tail == -1)
                    break;

                entries = (_pages[(_freeEntryList._tail)]._entries);
                Debug.Assert(((entries)[0]._cFree) > 0, "FreeEntryCount(entries) > 0");
                int availableFreeEntries = (_cPagesInUse * NUM_ENTRIES) - ((entries)[0]._cFree) - _cEntriesInUse;
                if (availableFreeEntries < (NUM_ENTRIES - ((entries)[0]._cFree)))
                    break;

                
                for (int i = 1; i < entries.Length; i++) {
                    
                    if (entries[i]._cacheEntry == null)
                        continue;

                    
                    Debug.Assert(_freeEntryList._head != _freeEntryList._tail, "_freeEntryList._head != _freeEntryList._tail");
                    ExpiresEntryRef newRef = GetFreeExpiresEntry();
                    Debug.Assert(newRef.PageIndex != _freeEntryList._tail, "newRef.PageIndex != _freeEntryList._tail");

                    
                    CacheEntry cacheEntry = entries[i]._cacheEntry;




#line 671 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"

                    cacheEntry.ExpiresEntryRef = newRef;

                    
                    ExpiresEntry[] newEntries = (_pages[(newRef.PageIndex)]._entries);
                    newEntries[newRef.Index] = entries[i];

                    
                    
                    
                    ((entries)[0]._cFree)++;
                }

                
                RemovePage(_freeEntryList._tail);

                Debug.Validate("CacheValidateExpires", this);
            }
        }

        
        
        internal void AddCacheEntry(CacheEntry cacheEntry) {
            lock (this) {
                
                
                
                if ((cacheEntry.State & (CacheEntry.EntryState.AddedToCache | CacheEntry.EntryState.AddingToCache)) == 0)
                    return;

                
                ExpiresEntryRef entryRef = cacheEntry.ExpiresEntryRef;
                Debug.Assert((cacheEntry.ExpiresBucket == 0xff) == entryRef.IsInvalid, "(cacheEntry.ExpiresBucket == 0xff) == entryRef.IsInvalid");
                if (cacheEntry.ExpiresBucket != 0xff || !entryRef.IsInvalid)
                    return;

                
                if (_freeEntryList._head == -1) {
                    Expand();
                }

                
                ExpiresEntryRef freeRef = GetFreeExpiresEntry();
                Debug.Assert(cacheEntry.ExpiresBucket == 0xff, "cacheEntry.ExpiresBucket == 0xff");
                Debug.Assert(cacheEntry.ExpiresEntryRef.IsInvalid, "cacheEntry.ExpiresEntryRef.IsInvalid");
                cacheEntry.ExpiresBucket = _bucket;
                cacheEntry.ExpiresEntryRef = freeRef;

                
                ExpiresEntry[] entries = (_pages[(freeRef.PageIndex)]._entries);
                int entryIndex = freeRef.Index;
                entries[entryIndex]._cacheEntry = cacheEntry;
                entries[entryIndex]._utcExpires = cacheEntry.UtcExpires;

                
                AddCount(cacheEntry.UtcExpires);

                _cEntriesInUse++;













#line 743 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"

                
                
                
                
                
                
                if ((cacheEntry.State & (CacheEntry.EntryState.AddedToCache | CacheEntry.EntryState.AddingToCache)) == 0) {
                    RemoveCacheEntryNoLock(cacheEntry);
                }
            }
        }

        
        void RemoveCacheEntryNoLock(CacheEntry cacheEntry) {
            ExpiresEntryRef entryRef = cacheEntry.ExpiresEntryRef;
            if (cacheEntry.ExpiresBucket != _bucket || entryRef.IsInvalid)
                return;

            ExpiresEntry[]  entries = (_pages[(entryRef.PageIndex)]._entries);
            int             entryIndex = entryRef.Index;



#line 768 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"

            
            RemoveCount(entries[entryIndex]._utcExpires);

            
            cacheEntry.ExpiresBucket = 0xff;
            cacheEntry.ExpiresEntryRef = ExpiresEntryRef.INVALID;
            entries[entryIndex]._cacheEntry = null;

            
            AddExpiresEntryToFreeList(entryRef);

            
            if (_cEntriesInUse == 0) {
                ResetCounts(DateTime.UtcNow);
            }

            
            Reduce();

            Debug.Trace("CacheExpiresRemove", 
                        "Removed item=" + cacheEntry.Key + 
                        ",_bucket=" + _bucket + 
                        ",ref=" + entryRef + 
                        ",now=" + Debug.FormatLocalDate(DateTime.Now) +
                        ",expires=" + DateTimeUtil.ConvertToLocalTime(cacheEntry.UtcExpires));


            Debug.Validate("CacheValidateExpires", this);
            Debug.Dump("CacheExpiresRemove", this);
        }

        
        internal void RemoveCacheEntry(CacheEntry cacheEntry) {
            lock (this) {
                RemoveCacheEntryNoLock(cacheEntry);
            }
        }

        
        internal void UtcUpdateCacheEntry(CacheEntry cacheEntry, DateTime utcExpires) {
            lock (this) {
                ExpiresEntryRef entryRef = cacheEntry.ExpiresEntryRef;
                if (cacheEntry.ExpiresBucket != _bucket || entryRef.IsInvalid)
                    return;

                ExpiresEntry[]  entries = (_pages[(entryRef.PageIndex)]._entries);
                int             entryIndex = entryRef.Index;

                Debug.Assert(cacheEntry == entries[entryIndex]._cacheEntry);

                
                RemoveCount(entries[entryIndex]._utcExpires);
                AddCount(utcExpires);

                
                entries[entryIndex]._utcExpires = utcExpires;

                
                cacheEntry.UtcExpires = utcExpires;

                Debug.Validate("CacheValidateExpires", this);
                Debug.Trace("CacheExpiresUpdate", "Updated item " + cacheEntry.Key + " in bucket " + _bucket);
            }
        }

        
        internal int FlushExpiredItems(DateTime utcNow, bool useInsertBlock) {
            
            if (_cEntriesInUse == 0 || GetExpiresCount(utcNow) == 0)
                return 0;

            Debug.Assert(_cEntriesInFlush == 0, "_cEntriesInFlush == 0");

            
            
            ExpiresEntryRef inFlushHead = ExpiresEntryRef.INVALID;

            ExpiresEntry[] entries;
            int entryIndex;
            CacheEntry cacheEntry;
            int flushed = 0;

            try {
                if (useInsertBlock) {
                    
                    _cacheExpires.CacheSingle.BlockInsertIfNeeded();
                }
                
                lock (this) {
                    Debug.Assert(_blockReduce == false, "_blockReduce == false");

                    
                    if (_cEntriesInUse == 0 || GetExpiresCount(utcNow) == 0)
                        return 0;




#line 868 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"

                    
                    
                    ResetCounts(utcNow);
                    int cPages = _cPagesInUse;
                    for (int i = 0; i < _pages.Length; i++) {
                        entries = _pages[i]._entries;
                        if (entries != null) {
                            int cEntries = NUM_ENTRIES - ((entries)[0]._cFree);
                            for (int j = 1; j < entries.Length; j++) {
                                cacheEntry = entries[j]._cacheEntry;
                                if (cacheEntry != null) {
                                    if (entries[j]._utcExpires > utcNow) {
                                        AddCount(entries[j]._utcExpires);
                                    }
                                    else {
                                        
                                        
                                        
                                        cacheEntry.ExpiresBucket = 0xff;
                                        cacheEntry.ExpiresEntryRef = ExpiresEntryRef.INVALID;

                                        
                                        entries[j]._cFree = 1;

                                        
                                        entries[j]._next = inFlushHead;
                                        inFlushHead = new ExpiresEntryRef(i, j);

                                        flushed++;
                                        _cEntriesInFlush++;
                                    }

                                    cEntries--;
                                    if (cEntries == 0)
                                        break;
                                }
                            }

                            cPages--;
                            if (cPages == 0)
                                break;
                        }
                    }

                    if (flushed == 0) {
                        Debug.Trace("CacheExpiresFlushTotal", "FlushExpiredItems flushed " + flushed +
                                    " expired items, bucket=" + _bucket + "; Time=" + Debug.FormatLocalDate(DateTime.Now));

                        return 0;
                    }

                    
                    _blockReduce = true;
                }
            }
            finally {
                if (useInsertBlock) {
                    
                    
                    
                    
                    
                    
                    _cacheExpires.CacheSingle.UnblockInsert();
                }
            }

            Debug.Assert(!inFlushHead.IsInvalid, "!inFlushHead.IsInvalid");

            
            CacheSingle cacheSingle = _cacheExpires.CacheSingle;
            ExpiresEntryRef current = inFlushHead;
            ExpiresEntryRef next;
            while (!current.IsInvalid) {
                entries = (_pages[(current.PageIndex)]._entries);
                entryIndex = current.Index;

                next = entries[entryIndex]._next;
                
                
                cacheEntry = entries[entryIndex]._cacheEntry;
                entries[entryIndex]._cacheEntry = null;
                Debug.Assert(cacheEntry.ExpiresEntryRef.IsInvalid, "cacheEntry.ExpiresEntryRef.IsInvalid");
                cacheSingle.Remove(cacheEntry, CacheItemRemovedReason.Expired);

                
                current = next;
            }

            try {
                if (useInsertBlock) {
                    
                    _cacheExpires.CacheSingle.BlockInsertIfNeeded();
                }

                lock (this) {
                    
                    current = inFlushHead;
                    while (!current.IsInvalid) {
                        entries = (_pages[(current.PageIndex)]._entries);
                        entryIndex = current.Index;

                        next = entries[entryIndex]._next;

                        _cEntriesInFlush--;
                        AddExpiresEntryToFreeList(current);

                        
                        current = next;
                    }

                    
                    Debug.Assert(_cEntriesInFlush == 0, "_cEntriesInFlush == 0");
                    _blockReduce = false;
                    Reduce();

                    Debug.Trace("CacheExpiresFlushTotal", "FlushExpiredItems flushed " + flushed +
                                " expired items, bucket=" + _bucket + "; Time=" + Debug.FormatLocalDate(DateTime.Now));

                    Debug.Validate("CacheValidateExpires", this);
                    Debug.Dump("CacheExpiresFlush", this);
                }
            }
            finally {
                if (useInsertBlock) {
                    _cacheExpires.CacheSingle.UnblockInsert();
                }
            }

            return flushed;
        }





































































































































#line 1134 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"
    }

    
    





    sealed class CacheExpires {
        internal static readonly TimeSpan   MIN_UPDATE_DELTA = new TimeSpan(0, 0, 1);
        internal static readonly TimeSpan   MIN_FLUSH_INTERVAL = new TimeSpan(0, 0, 1);
        internal static readonly TimeSpan   _tsPerBucket = new TimeSpan(0, 0, 20);

        const int                   NUMBUCKETS = 30;
        static readonly TimeSpan    _tsPerCycle = new TimeSpan(NUMBUCKETS * _tsPerBucket.Ticks);

        readonly CacheSingle      _cacheSingle;
        readonly ExpiresBucket[]    _buckets;
        DisposableGCHandleRef<Timer> _timerHandleRef;
        DateTime                    _utcLastFlush;
        int                         _inFlush;


        internal CacheExpires(CacheSingle cacheSingle) {
            Debug.Assert(NUMBUCKETS < Byte.MaxValue);

            DateTime utcNow = DateTime.UtcNow;

            _cacheSingle = cacheSingle;
            _buckets = new ExpiresBucket[NUMBUCKETS];
            for (byte b = 0; b < _buckets.Length; b++) {
                _buckets[b] = new ExpiresBucket(this, b, utcNow);
            }
        }

        int UtcCalcExpiresBucket(DateTime utcDate) {
            long    ticksFromCycleStart = utcDate.Ticks % _tsPerCycle.Ticks;
            int     bucket = (int) (((ticksFromCycleStart / _tsPerBucket.Ticks) + 1) % NUMBUCKETS);

            return bucket;
        }

        int FlushExpiredItems(bool checkDelta, bool useInsertBlock) {
            int flushed = 0;

            if (Interlocked.Exchange(ref _inFlush, 1) == 0) {
                try {
                    
                    if (_timerHandleRef == null) {
                        return 0;
                    }
                    DateTime utcNow = DateTime.UtcNow;
                    if (!checkDelta || utcNow - _utcLastFlush >= MIN_FLUSH_INTERVAL || utcNow < _utcLastFlush) {
                        _utcLastFlush = utcNow;
                        foreach (ExpiresBucket bucket in _buckets) {
                            flushed += bucket.FlushExpiredItems(utcNow, useInsertBlock);
                        }

                        Debug.Trace("CacheExpiresFlushTotal", "FlushExpiredItems flushed a total of " + flushed + " items; Time=" + Debug.FormatLocalDate(DateTime.Now));
                        Debug.Dump("CacheExpiresFlush", this);
                    }

                }
                finally {
                    Interlocked.Exchange(ref _inFlush, 0);
                }
            }

            return flushed;
        }

        internal int FlushExpiredItems(bool useInsertBlock) {
            return FlushExpiredItems(true, useInsertBlock);
        }

        void TimerCallback(object state) {
            FlushExpiredItems(false, false);
        }

        internal void EnableExpirationTimer(bool enable) {




#line 1220 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"

            if (enable) {
                if (_timerHandleRef == null) {
                    DateTime utcNow = DateTime.UtcNow;
                    TimeSpan due = _tsPerBucket - (new TimeSpan(utcNow.Ticks % _tsPerBucket.Ticks));
                    Timer timer = new Timer(new TimerCallback(this.TimerCallback), null, 
                            due.Ticks / TimeSpan.TicksPerMillisecond, _tsPerBucket.Ticks / TimeSpan.TicksPerMillisecond);
                    _timerHandleRef = new DisposableGCHandleRef<Timer>(timer);

                    Debug.Trace("Cache", "Cache expiration timer created.");
                }
            }
            else {
                DisposableGCHandleRef<Timer> timerHandleRef = _timerHandleRef;
                if (timerHandleRef != null && Interlocked.CompareExchange(ref _timerHandleRef, null, timerHandleRef) == timerHandleRef) {
                    timerHandleRef.Dispose();
                    Debug.Trace("Cache", "Cache expiration timer disposed.");
                    while (_inFlush != 0) {
                        Thread.Sleep(100);
                    }
                }
            }
        }

        internal CacheSingle CacheSingle {
            get {
                return _cacheSingle;
            }
        }

        




        internal void Add(CacheEntry cacheEntry) {
            DateTime utcNow = DateTime.UtcNow;
            if (utcNow > cacheEntry.UtcExpires) {
                cacheEntry.UtcExpires = utcNow;
            }

            int bucket = UtcCalcExpiresBucket(cacheEntry.UtcExpires);
            _buckets[bucket].AddCacheEntry(cacheEntry);
        }

        




        internal void Remove(CacheEntry cacheEntry) {
            byte bucket = cacheEntry.ExpiresBucket;
            if (bucket != 0xff) {
                _buckets[bucket].RemoveCacheEntry(cacheEntry);
            }
        }

        



        internal void UtcUpdate(CacheEntry cacheEntry, DateTime utcNewExpires) {
            int oldBucket = cacheEntry.ExpiresBucket;
            int newBucket = UtcCalcExpiresBucket(utcNewExpires);
            
            if (oldBucket != newBucket) {
                Debug.Trace("CacheExpiresUpdate", 
                            "Updating item " + cacheEntry.Key + " from bucket " + oldBucket + " to new bucket " + newBucket);

                if (oldBucket != 0xff) {
                    _buckets[oldBucket].RemoveCacheEntry(cacheEntry);
                    cacheEntry.UtcExpires = utcNewExpires;
                    _buckets[newBucket].AddCacheEntry(cacheEntry);
                }
            } else {
                if (oldBucket != 0xff) {
                    _buckets[oldBucket].UtcUpdateCacheEntry(cacheEntry, utcNewExpires);
                }
            }
        }

























#line 1326 "C:\\GitHub\\WebFormsCore\\src\\System.Web\\cacheexpires.cspp"
    }
}


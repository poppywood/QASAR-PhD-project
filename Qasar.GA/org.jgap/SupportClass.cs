//
// In order to convert some functionality to Visual C#, the Java Language Conversion Assistant
// creates "support classes" that duplicate the original functionality.  
//
// Support classes replicate the functionality of the original code, but in some cases they are 
// substantially different architecturally. Although every effort is made to preserve the 
// original architecture of the application in the converted project, the user should be aware that 
// the primary goal of these support classes is to replicate functionality, and that at times 
// the architecture of the resulting solution may differ somewhat.
//
namespace org.jgap
{
	using System;

	/// <summary>
	/// Contains conversion support elements such as classes, interfaces and static methods.
	/// </summary>
	public class SupportClass
	{
		/// <summary>
		/// This class has static methods to manage collections.
		/// </summary>
		public class CollectionsManager
		{
			/// <summary>
			/// Initializes a new instance of the IList class that is empty.
			/// </summary>
			public static System.Collections.IList EMPTY_LIST = System.Collections.ArrayList.ReadOnly(new System.Collections.ArrayList());
		            
			/// <summary>
			/// Initializes a new instance of the IDictionary class that is empty.
			/// </summary>
			public static System.Collections.IDictionary EMPTY_MAP = new System.Collections.Hashtable();
		            
			/// <summary>
			/// Initializes a new instance of the ICollection class that is empty.
			/// </summary>
			public static System.Collections.ICollection EMPTY_SET = System.Collections.ArrayList.ReadOnly(new System.Collections.ArrayList());
		 
			/// <summary>
			/// Initializes a new instance of the CollectionsManager class that is empty.
			/// </summary>
			public CollectionsManager()
			{
			}
		            
			/// <summary>
			/// Copies the IList to other IList.
			/// </summary>
			/// <param name="SourceList">IList source.</param>
			/// <param name="TargetList">IList target.</param>
			public static void Copy(System.Collections.IList SourceList, System.Collections.IList TargetList)
			{
				for (int i = 0; i < SourceList.Count; i++)
					TargetList[i] = SourceList[i];
			}
		            
			/// <summary>
			/// Returns an enumerator to iterate through the Collection.
			/// </summary>
			/// <param name="Collection">Collection to obtain the IEnumerator.</param>
			/// <returns>An IEnumerator for the entire Collection.</returns>
			public static System.Collections.IEnumerator Enumeration(System.Collections.ICollection Collection)
			{
				System.Collections.ArrayList Enumerator = new System.Collections.ArrayList(Collection);
				return Enumerator.GetEnumerator();
			}
		 
			/// <summary>
			/// Replaces the elements of the specified list with the specified element.
			/// </summary>
			/// <param name="List">The list to be filled with the specified element.</param>
			/// <param name="Element">The element with which to fill the specified list.</param>
			public static void Fill(System.Collections.IList List, System.Object Element)
			{
				for(int i = 0; i < List.Count; i++)
					List[i] = Element;      
			}
		 
			/// <summary>
			/// Shuffles the list randomly.
			/// </summary>
			/// <param name="List">The list to be shuffled.</param>
			public static void Shuffle(System.Collections.IList List)
			{
				System.Random RandomList = new System.Random(unchecked((int)System.DateTime.Now.Ticks));
				Shuffle(List, RandomList);
			}

			/// <summary>
			/// Shuffles the list randomly.
			/// </summary>
			/// <param name="List">The list to be shuffled.</param>
			public static void Shuffle(System.Collections.ICollection List)
			{
				System.Random RandomList = new System.Random(unchecked((int)System.DateTime.Now.Ticks));
				Shuffle(List, RandomList);
			}
		 
			/// <summary>
			/// Shuffles the list randomly.
			/// </summary>
			/// <param name="List">The list to be shuffled.</param>
			/// <param name="RandomList">The random to use to shuffle the list.</param>
			public static void Shuffle(System.Collections.IList List, System.Random RandomList)
			{
				System.Object source = null;
				int  target = 0;
		 
				for (int i = 0; i < List.Count; i++)
				{
					target  = RandomList.Next(List.Count);
					source  = (System.Object)List[i];
					List[i] = List[target];
					List[target] = source;
				}
			}

			/// <summary>
			/// Shuffles the list randomly.
			/// </summary>
			/// <param name="List">The list to be shuffled.</param>
			/// <param name="RandomList">The random to use to shuffle the list.</param>
			public static void Shuffle(System.Collections.ICollection List, System.Random RandomList)
			{
				System.Object source = null;
				int  target = 0;
				System.Collections.Stack stack = new System.Collections.Stack(List.Count);
				Object[] ListInArray = new Object[List.Count];
				List.CopyTo(ListInArray,0);
		 			
				for (int i = 0; i < List.Count; i++)
				{
					target  = RandomList.Next(List.Count);
					source  = (System.Object)ListInArray[i];
					ListInArray[i] = ListInArray[target];
					ListInArray[target] = source;
				}
				for (int i = List.Count-1; i != -1; i--)
				{
					stack.Push(ListInArray[i]);
				}
				List = stack;
			}
		 
			/// <summary>
			/// Reverses the order of the elements in the ArrayList or a portion of it.
			/// </summary>
			/// <param name="List">IList that goes away has to reverse.</param>
			public static void Reverse(System.Collections.IList List)
			{
				System.Collections.ArrayList ReverseList = (System.Collections.ArrayList)List;
				ReverseList.Reverse();
				List = (System.Collections.IList)ReverseList;
			}
		 
			/// <summary>
			/// Uses a binary search algorithm to locate a specific element in the sorted ArrayList or a portion of it.
			/// </summary>
			/// <param name="List">IList in which one goes away has to make the binary search.</param>
			/// <param name="Key">The Object to locate.</param>
			/// <param name="Comparator">The IComparer implementation to use when comparing elements.</param>
			/// <returns></returns>
			public static int BinarySearch(System.Collections.IList List, System.Object Key, System.Collections.IComparer Comparator)
			{
				System.Collections.ArrayList SearchList = (System.Collections.ArrayList)List;
				return SearchList.BinarySearch(Key, Comparator);
			}

			/// <summary>
			/// Uses a binary search algorithm to locate a specific element in the sorted ArrayList or a portion of it.
			/// </summary>
			/// <param name="List">IList in which one goes away has to make the binary search.</param>
			/// <param name="Key">The Object to locate.</param>
			/// <param name="Comparator">The IComparer implementation to use when comparing elements.</param>
			/// <returns></returns>
			public static int BinarySearch(System.Collections.ICollection List, System.Object Key, System.Collections.IComparer Comparator)
			{
				System.Collections.ArrayList SearchList = new System.Collections.ArrayList(List);
				return SearchList.BinarySearch(Key, Comparator);
			}
		 
			/// <summary>
			/// Returns an IList whose elements are copies of the specified value.
			/// </summary>
			/// <param name="loop">The number of times value should be copied.</param>
			/// <param name="Element">The Object to copy multiple times in the new IList.</param>
			/// <returns>An IList with count number of elements, all of which are copies of value.</returns>
			public static System.Collections.IList NCopies(int loop, System.Object Element)
			{
				return (System.Collections.IList)(System.Collections.ArrayList.ReadOnly(System.Collections.ArrayList.Repeat(Element, loop)));
			}
		 
			/// <summary>
			/// Sorts the elements in the entire ArrayList using the IComparable implementation of each element.
			/// </summary>
			/// <param name="List">ArrayList that wishes to order.</param>
			/// <param name="Comparator">The IComparer implementation to use when comparing elements. </param>
			public static void Sort(System.Collections.IList List,System.Collections.IComparer Comparator)
			{
				System.Collections.ArrayList SortList = (System.Collections.ArrayList)List;
				SortList.Sort(Comparator);
				List = SortList;
			}     

			/// <summary>
			/// Sorts the elements in the entire ArrayList using the IComparable implementation of each element.
			/// </summary>
			/// <param name="List">ArrayList that wishes to order.</param>
			/// <param name="Comparator">The IComparer implementation to use when comparing elements. </param>
			public static void Sort(System.Collections.ICollection List,System.Collections.IComparer Comparator)
			{
				System.Collections.ArrayList SortList = new System.Collections.ArrayList(List);			
				if (Comparator == null)
					SortList.Sort();
				else
					SortList.Sort(Comparator);
				List = SortList;
			}     

		            
			/// <summary>
			/// This class implements System.Collections.IComparer and is used for Comparing two String objects by evaluating 
			/// the numeric values of the corresponding Char objects in each string.
			/// </summary>
			class CompareCharValues : System.Collections.IComparer
			{
				public int Compare(object x, object y)
				{
					return string.CompareOrdinal((string)x, (string)y);
				}
			}

			/// <summary>
			/// Obtain the maximum element of the given collection with the specified comparator.
			/// </summary>
			/// <param name="Collection">Collection from which the maximum value will be obtained.</param>
			/// <param name="Comparator">The comparator with which to determine the maximum element.</param>
			/// <returns></returns>
			public static System.Object Max(System.Collections.ICollection Collection, System.Collections.IComparer Comparator)
			{
				if ((Comparator == null) || (Comparator is System.Collections.Comparer)) 
				{
					CompareCharValues charComparer = new CompareCharValues();
					System.Collections.ArrayList SortCollection = new System.Collections.ArrayList(Collection);
					SortCollection.Sort(charComparer);
					return (System.Object)SortCollection[SortCollection.Count - 1];
				}
				else
				{
					System.Collections.ArrayList SortCollection = new System.Collections.ArrayList(Collection);
					SortCollection.Sort(Comparator);
					return (System.Object)SortCollection[SortCollection.Count - 1];
				}
			}

			/// <summary>
			/// Obtain the minimum element of the given collection with the specified comparator.
			/// </summary>
			/// <param name="Collection">Collection from which the minimum value will be obtained.</param>
			/// <param name="Comparator">The comparator with which to determine the minimum element.</param>
			/// <returns></returns>
			public static System.Object Min(System.Collections.ICollection Collection, System.Collections.IComparer Comparator)
			{
				if ((Comparator == null) || (Comparator is System.Collections.Comparer)) 
				{
					CompareCharValues charComparer = new CompareCharValues();
					System.Collections.ArrayList SortCollection = new System.Collections.ArrayList(Collection);
					SortCollection.Sort(charComparer);
					return (System.Object)SortCollection[0];
				}
				else
				{
					System.Collections.ArrayList SortCollection = new System.Collections.ArrayList(Collection);
					SortCollection.Sort(Comparator);
					return (System.Object)SortCollection[0];
				}
			}
		 
			/// <summary>
			/// Returns an ICollection wrapper that is synchronized (thread-safe).
			/// </summary>
			/// <param name="Set">The ICollection to synchronize.</param>
			/// <returns>An ICollection wrapper that is synchronized (thread-safe).</returns>
			public static System.Collections.ICollection SynchronizedSet(System.Collections.ICollection Set)
			{
				return System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(Set));
			}
		 
			/// <summary>
			/// Returns an ICollection wrapper that is synchronized (thread-safe).
			/// </summary>
			/// <param name="Set">The ICollection to synchronize.</param>
			/// <returns>An ICollection wrapper that is synchronized (thread-safe).</returns>
			public static System.Collections.ICollection SynchronizedCollection(System.Collections.ICollection Collection)
			{
				return System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(Collection));
			}
		 
			/// <summary>
			/// Returns an IList wrapper that is synchronized (thread-safe).
			/// </summary>
			/// <param name="List">The IList to synchronize.</param>
			/// <returns>An IList wrapper that is synchronized (thread-safe).</returns>
			public static System.Collections.IList SynchronizedList(System.Collections.IList List)
			{
				return System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(List));
			}
		 
			/// <summary>
			/// Returns an IDictionary wrapper that is synchronized (thread-safe).
			/// </summary>
			/// <param name="Map">The IDictionary to synchronize.</param>
			/// <returns>An IDictionary wrapper that is synchronized (thread-safe).</returns>
			public static System.Collections.IDictionary SynchronizedMap(System.Collections.IDictionary Map)
			{
				return (System.Collections.IDictionary)System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(Map));
			}
		 
			/// <summary>
			/// Creates and returns a read-only System.Collections.SortedList.
			/// </summary>
			/// <param name="SortedMap">System.Collections.SortedList with which a new System.Collections.SortedList will be created.</param>
			/// <returns>Returns a sortedlist wrapper that is synchronized (thread-safe).</returns>
			public static System.Collections.SortedList SynchronizedSortedMap(System.Collections.SortedList SortedMap)
			{
				return System.Collections.SortedList.Synchronized(new System.Collections.SortedList(SortedMap));
			}
		 
			/// <summary>
			/// Creates and returns a read-only System.Collections.ICollection.
			/// </summary>
			/// <param name="Set">System.Collections.ICollection with which a new System.Collections.ICollection will be created.</param>
			/// <returns>Return a read-only System.Collections.ICollection, containing the specified ICollection.</returns>
			public static System.Collections.ICollection UnModifiableSet(System.Collections.ICollection Set)
			{
				return System.Collections.ArrayList.ReadOnly(new System.Collections.ArrayList(Set));
			}
		 
			/// <summary>
			/// Creates and returns a read-only System.Collections.ICollection.
			/// </summary>
			/// <param name="Collection">System.Collections.ICollection with which a new System.Collections.ICollection will be created.</param>
			/// <returns>Return a read-only System.Collections.ICollection, containing the specified ICollection.</returns>
			public static System.Collections.ICollection UnModifiableCollection(System.Collections.ICollection Collection)
			{
				return System.Collections.ArrayList.ReadOnly(new System.Collections.ArrayList(Collection));
			}
		 
			/// <summary>
			/// Creates and returns a read-only System.Collections.IList.
			/// </summary>
			/// <param name="List">System.Collections.IList with which a new System.Collections.ICollection will be created.</param>
			/// <returns>Return a read-only System.Collections.IList, containing the specified IList.</returns>
			public static System.Collections.IList UnModifiableList(System.Collections.IList List)
			{
				return System.Collections.ArrayList.ReadOnly(new System.Collections.ArrayList(List));
			}
		 
			/// <summary>
			/// Creates and returns a read-only System.Collections.IDictionary.
			/// </summary>
			/// <param name="Map">System.Collections.IDictionary with which a new System.Collections.ICollection will be created.</param>
			/// <returns>Return a read-only System.Collections.IDictionary, containing the specified IDictionary.</returns>
			public static System.Collections.IDictionary UnModifiableMap(System.Collections.IDictionary Map)
			{
				return (System.Collections.IDictionary)System.Collections.ArrayList.ReadOnly(new System.Collections.ArrayList(Map));
			}
		 
			/// <summary>
			/// Creates and returns a read-only System.Collections.ICollection.
			/// </summary>
			/// <param name="Element">Element with which System.Collections.ICollection will be created.</param>
			/// <returns>Return a read-only System.Collections.ICollection, containing only the specified object.</returns>
			public static System.Collections.ICollection Singleton(System.Object Element)
			{
				System.Collections.ICollection Collection = (System.Collections.ICollection)new System.Object[]{Element};
				return System.Collections.ArrayList.ReadOnly(new System.Collections.ArrayList(Collection));
			}
		 
			/// <summary>
			/// Creates and returns a read-only System.Collections.IList.
			/// </summary>
			/// <param name="Element">Element with which System.Collections.IList will be created.</param>
			/// <returns>Return a read-only System.Collections.IList, containing only the specified object.</returns>
			public static System.Collections.IList SingletonList(System.Object Element)
			{
				System.Collections.ICollection Collection = (System.Collections.ICollection)new System.Object[]{Element};
				return System.Collections.ArrayList.ReadOnly(new System.Collections.ArrayList(Collection));
			}
		 
			/// <summary>
			/// Creates and returns a read-only System.Collections.IDictionary.
			/// </summary>
			/// <param name="Element">Element with which System.Collections.IDictionary will be created.</param>
			/// <returns>Return a read-only System.Collections.IDictionary, containing only the specified object.</returns>
			public static System.Collections.IDictionary SingletonMap(System.Object Element)
			{
				System.Collections.ICollection Collection = (System.Collections.ICollection)new System.Object[]{Element};
				return (System.Collections.IDictionary)System.Collections.ArrayList.ReadOnly(new System.Collections.ArrayList(Collection));
			}
		}


		/*******************************/
		/// <summary>
		/// This class contains different methods to manage Collections.
		/// </summary>
		public class CollectionSupport : System.Collections.CollectionBase
		{
			/// <summary>
			/// Creates an instance of the Collection by using an inherited constructor.
			/// </summary>
			public CollectionSupport() : base()
			{			
			}

			/// <summary>
			/// Adds an specified element to the collection.
			/// </summary>
			/// <param name="element">The element to be added.</param>
			/// <returns>Returns true if the element was successfuly added. Otherwise returns false.</returns>
			public virtual bool Add(System.Object element)
			{
				return (this.List.Add(element) != -1);
			}	

			/// <summary>
			/// Adds all the elements contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(System.Collections.ICollection collection)
			{
				bool result = false;
				if (collection!=null)
				{
					System.Collections.IEnumerator tempEnumerator = new System.Collections.ArrayList(collection).GetEnumerator();
					while (tempEnumerator.MoveNext())
					{
						if (tempEnumerator.Current != null)
							result = this.Add(tempEnumerator.Current);
					}
				}
				return result;
			}


			/// <summary>
			/// Adds all the elements contained in the specified support class collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(CollectionSupport collection)
			{
				return this.AddAll((System.Collections.ICollection)collection);
			}

			/// <summary>
			/// Verifies if the specified element is contained into the collection. 
			/// </summary>
			/// <param name="element"> The element that will be verified.</param>
			/// <returns>Returns true if the element is contained in the collection. Otherwise returns false.</returns>
			public virtual bool Contains(System.Object element)
			{
				return this.List.Contains(element);
			}

			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(System.Collections.ICollection collection)
			{
				bool result = false;
				System.Collections.IEnumerator tempEnumerator = new System.Collections.ArrayList(collection).GetEnumerator();
				while (tempEnumerator.MoveNext())
					if (!(result = this.Contains(tempEnumerator.Current)))
						break;
				return result;
			}

			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(CollectionSupport collection)
			{
				return this.ContainsAll((System.Collections.ICollection) collection);
			}

			/// <summary>
			/// Verifies if the collection is empty.
			/// </summary>
			/// <returns>Returns true if the collection is empty. Otherwise returns false.</returns>
			public virtual bool IsEmpty()
			{
				return (this.Count == 0);
			}

			/// <summary>
			/// Removes an specified element from the collection.
			/// </summary>
			/// <param name="element">The element to be removed.</param>
			/// <returns>Returns true if the element was successfuly removed. Otherwise returns false.</returns>
			public virtual bool Remove(System.Object element)
			{
				bool result = false;
				if (this.Contains(element))
				{
					this.List.Remove(element);
					result = true;
				}
				return result;
			}

			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(System.Collections.ICollection collection)
			{ 
				bool result = false;
				System.Collections.IEnumerator tempEnumerator = new System.Collections.ArrayList(collection).GetEnumerator();
				while (tempEnumerator.MoveNext())
				{
					if (this.Contains(tempEnumerator.Current))
						result = this.Remove(tempEnumerator.Current);
				}
				return result;
			}

			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(CollectionSupport collection)
			{ 
				return this.RemoveAll((System.Collections.ICollection) collection);
			}

			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(System.Collections.ICollection collection)
			{
				bool result = false;
				System.Collections.IEnumerator tempEnumerator = this.GetEnumerator();
				CollectionSupport tempCollection = new CollectionSupport();
				tempCollection.AddAll(collection);
				while (tempEnumerator.MoveNext())
					if (!tempCollection.Contains(tempEnumerator.Current))
					{
						result = this.Remove(tempEnumerator.Current);
					
						if (result == true)
						{
							tempEnumerator = this.GetEnumerator();
						}
					}
				return result;
			}

			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(CollectionSupport collection)
			{
				return this.RetainAll((System.Collections.ICollection) collection);
			}

			/// <summary>
			/// Obtains an array containing all the elements of the collection.
			/// </summary>
			/// <returns>The array containing all the elements of the collection</returns>
			public virtual System.Object[] ToArray()
			{	
				int index = 0;
				System.Object[] objects = new System.Object[this.Count];
				System.Collections.IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					objects[index++] = tempEnumerator.Current;
				return objects;
			}

			/// <summary>
			/// Obtains an array containing all the elements of the collection.
			/// </summary>
			/// <param name="objects">The array into which the elements of the collection will be stored.</param>
			/// <returns>The array containing all the elements of the collection.</returns>
			public virtual System.Object[] ToArray(System.Object[] objects)
			{	
				int index = 0;
				System.Collections.IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					objects[index++] = tempEnumerator.Current;
				return objects;
			}

			/// <summary>
			/// Creates a CollectionSupport object with the contents specified in array.
			/// </summary>
			/// <param name="array">The array containing the elements used to populate the new CollectionSupport object.</param>
			/// <returns>A CollectionSupport object populated with the contents of array.</returns>
			public static CollectionSupport ToCollectionSupport(System.Object[] array)
			{
				CollectionSupport tempCollectionSupport = new CollectionSupport();             
				tempCollectionSupport.AddAll(array);
				return tempCollectionSupport;
			}
		}

		/*******************************/
		/// <summary>
		/// This class contains different methods to manage list collections.
		/// </summary>
		public class ListCollectionSupport : System.Collections.ArrayList
		{
			/// <summary>
			/// Creates a new instance of the class ListCollectionSupport.
			/// </summary>
			public ListCollectionSupport() : base()
			{
			}
 
			/// <summary>
			/// Creates a new instance of the class ListCollectionSupport.
			/// </summary>
			/// <param name="collection">The collection to insert into the new object.</param>
			public ListCollectionSupport(System.Collections.ICollection collection) : base(collection)
			{
			}

			/// <summary>
			/// Creates a new instance of the class ListCollectionSupport with the specified capacity.
			/// </summary>
			/// <param name="capacity">The capacity of the new array.</param>
			public ListCollectionSupport(int capacity) : base(capacity)
			{
			}

			/// <summary>
			/// Adds an object to the end of the List.
			/// </summary>          
			/// <param name="valueToInsert">The value to insert in the array list.</param>
			/// <returns>Returns true after adding the value.</returns>
			public virtual bool Add(System.Object valueToInsert)
			{
				base.Insert(this.Count, valueToInsert);
				return true;
			}

			/// <summary>
			/// Adds all the elements contained into the specified collection, starting at the specified position.
			/// </summary>
			/// <param name="index">Position at which to add the first element from the specified collection.</param>
			/// <param name="list">The list used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(int index, System.Collections.IList list)
			{
				bool result = false;
				if (list!=null)
				{
					System.Collections.IEnumerator tempEnumerator = new System.Collections.ArrayList(list).GetEnumerator();
					int tempIndex = index;
					while (tempEnumerator.MoveNext())
					{
						base.Insert(tempIndex++, tempEnumerator.Current);
						result = true;
					}
				}
				return result;
			}

			/// <summary>
			/// Adds all the elements contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(System.Collections.IList collection)
			{
				return this.AddAll(this.Count,collection);
			}

			/// <summary>
			/// Adds all the elements contained in the specified support class collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(CollectionSupport collection)
			{
				return this.AddAll(this.Count,collection);
			}

			/// <summary>
			/// Adds all the elements contained into the specified support class collection, starting at the specified position.
			/// </summary>
			/// <param name="index">Position at which to add the first element from the specified collection.</param>
			/// <param name="list">The list used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(int index, CollectionSupport collection)
			{
				return this.AddAll(index,(System.Collections.IList)collection);
			}
		
			/// <summary>
			/// Creates a copy of the ListCollectionSupport.
			/// </summary>
			/// <returns> A copy of the ListCollectionSupport.</returns>
			public virtual System.Object ListCollectionClone()
			{
				return MemberwiseClone();
			}


			/// <summary>
			/// Returns an iterator of the collection.
			/// </summary>
			/// <returns>An IEnumerator.</returns>
			public virtual System.Collections.IEnumerator ListIterator()
			{
				return base.GetEnumerator();
			}

			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(System.Collections.ICollection collection)
			{ 
				bool result = false;
				System.Collections.IEnumerator tempEnumerator = new System.Collections.ArrayList(collection).GetEnumerator();
				while (tempEnumerator.MoveNext())
				{
					result = true;
					if (base.Contains(tempEnumerator.Current))
						base.Remove(tempEnumerator.Current);
				}
				return result;
			}
		
			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(CollectionSupport collection)
			{ 
				return this.RemoveAll((System.Collections.ICollection) collection);
			}		

			/// <summary>
			/// Removes the value in the specified index from the list.
			/// </summary>          
			/// <param name="index">The index of the value to remove.</param>
			/// <returns>Returns the value removed.</returns>
			public virtual System.Object RemoveElement(int index)
			{
				System.Object objectRemoved = this[index];
				this.RemoveAt(index);
				return objectRemoved;
			}

			/// <summary>
			/// Removes an specified element from the collection.
			/// </summary>
			/// <param name="element">The element to be removed.</param>
			/// <returns>Returns true if the element was successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveElement(System.Object element)
			{

				bool result = false;
				if (this.Contains(element))
				{
					base.Remove(element);
					result = true;
				}
				return result;
			}

			/// <summary>
			/// Removes the first value from an array list.
			/// </summary>          
			/// <returns>Returns the value removed.</returns>
			public virtual System.Object RemoveFirst()
			{
				System.Object objectRemoved = this[0];
				this.RemoveAt(0);
				return objectRemoved;
			}

			/// <summary>
			/// Removes the last value from an array list.
			/// </summary>
			/// <returns>Returns the value removed.</returns>
			public virtual System.Object RemoveLast()
			{
				System.Object objectRemoved = this[this.Count-1];
				base.RemoveAt(this.Count-1);
				return objectRemoved;
			}

			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(System.Collections.ICollection collection)
			{
				bool result = false;
				System.Collections.IEnumerator tempEnumerator = this.GetEnumerator();
				ListCollectionSupport tempCollection = new ListCollectionSupport(collection);
				while (tempEnumerator.MoveNext())
					if (!tempCollection.Contains(tempEnumerator.Current))
					{
						result = this.RemoveElement(tempEnumerator.Current);
					
						if (result == true)
						{
							tempEnumerator = this.GetEnumerator();
						}
					}
				return result;
			}
		
			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(CollectionSupport collection)
			{
				return this.RetainAll((System.Collections.ICollection) collection);
			}		

			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(System.Collections.ICollection collection)
			{
				bool result = false;
				System.Collections.IEnumerator tempEnumerator = new System.Collections.ArrayList(collection).GetEnumerator();
				while (tempEnumerator.MoveNext())
					if(!(result = this.Contains(tempEnumerator.Current)))
						break;
				return result;
			}
		
			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(CollectionSupport collection)
			{
				return this.ContainsAll((System.Collections.ICollection) collection);
			}		

			/// <summary>
			/// Returns a new list containing a portion of the current list between a specified range. 
			/// </summary>
			/// <param name="startIndex">The start index of the range.</param>
			/// <param name="endIndex">The end index of the range.</param>
			/// <returns>A ListCollectionSupport instance containing the specified elements.</returns>
			public virtual ListCollectionSupport SubList(int startIndex, int endIndex)
			{
				int index = 0;
				System.Collections.IEnumerator tempEnumerator = this.GetEnumerator();
				ListCollectionSupport result = new ListCollectionSupport();
				for(index = startIndex; index < endIndex; index++)
					result.Add(this[index]);
				return (ListCollectionSupport)result;
			}

			/// <summary>
			/// Obtains an array containing all the elements of the collection.
			/// </summary>
			/// <param name="objects">The array into which the elements of the collection will be stored.</param>
			/// <returns>The array containing all the elements of the collection.</returns>
			public virtual System.Object[] ToArray(System.Object[] objects)
			{	
				if (objects.Length < this.Count)
					objects = new System.Object[this.Count];
				int index = 0;
				System.Collections.IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					objects[index++] = tempEnumerator.Current;
				return objects;
			}

			/// <summary>
			/// Returns an iterator of the collection starting at the specified position.
			/// </summary>
			/// <param name="index">The position to set the iterator.</param>
			/// <returns>An IEnumerator at the specified position.</returns>
			public virtual System.Collections.IEnumerator ListIterator(int index)
			{
				if ((index < 0) || (index > this.Count)) throw new System.IndexOutOfRangeException();			
				System.Collections.IEnumerator tempEnumerator= this.GetEnumerator();
				if (index > 0)
				{
					int i=0;
					while ((tempEnumerator.MoveNext()) && (i < index - 1))
						i++;
				}
				return tempEnumerator;			
			}
	
			/// <summary>
			/// Gets the last value from a list.
			/// </summary>
			/// <returns>Returns the last element of the list.</returns>
			public virtual System.Object GetLast()
			{
				if (this.Count == 0) throw new System.ArgumentOutOfRangeException();
				else
				{
					return this[this.Count - 1];
				}									 
			}
		
			/// <summary>
			/// Return whether this list is empty.
			/// </summary>
			/// <returns>True if the list is empty, false if it isn't.</returns>
			public virtual bool IsEmpty()
			{
				return (this.Count == 0);
			}
		
			/// <summary>
			/// Replaces the element at the specified position in this list with the specified element.
			/// </summary>
			/// <param name="index">Index of element to replace.</param>
			/// <param name="element">Element to be stored at the specified position.</param>
			/// <returns>The element previously at the specified position.</returns>
			public virtual System.Object Set(int index, System.Object element)
			{
				System.Object result = this[index];
				this[index] = element;
				return result;
			} 

			/// <summary>
			/// Returns the element at the specified position in the list.
			/// </summary>
			/// <param name="index">Index of element to return.</param>
			/// <param name="element">Element to be stored at the specified position.</param>
			/// <returns>The element at the specified position in the list.</returns>
			public virtual System.Object Get(int index)
			{
				return this[index];
			}
		}

		/*******************************/
		/// <summary>
		/// Determines whether two Collections instances are equals.
		/// </summary>
		/// <param name="source">The first Collections to compare. </param>
		/// <param name="target">The second Collections to compare. </param>
		/// <returns>Return true if the first collection is the same instance as the second collection, otherwise return false.</returns>
		public static bool EqualsSupport(System.Collections.ICollection source, System.Collections.ICollection target )
		{
			System.Collections.IEnumerator sourceEnumerator = ReverseStack(source);
			System.Collections.IEnumerator targetEnumerator = ReverseStack(target);
     
			if (source.Count != target.Count)
				return false;
			while(sourceEnumerator.MoveNext() && targetEnumerator.MoveNext())
				if (!sourceEnumerator.Current.Equals(targetEnumerator.Current))
					return false;
			return true;
		}
	
		/// <summary>
		/// Determines if a Collection is equal to the Object.
		/// </summary>
		/// <param name="source">The first Collections to compare.</param>
		/// <param name="target">The Object to compare.</param>
		/// <returns>Return true if the first collection contains the same values of the second Object, otherwise return false.</returns>
		public static bool EqualsSupport(System.Collections.ICollection source, System.Object target)
		{
			if((target.GetType())!= (typeof(System.Collections.ICollection)))
				return false;
			else
				return EqualsSupport(source,(System.Collections.ICollection)target);
		}

		/// <summary>
		/// Determines if a IDictionaryEnumerator is equal to the Object.
		/// </summary>
		/// <param name="source">The first IDictionaryEnumerator to compare.</param>
		/// <param name="target">The second Object to compare.</param>
		/// <returns>Return true if the first IDictionaryEnumerator contains the same values of the second Object, otherwise return false.</returns>
		public static bool EqualsSupport(System.Collections.IDictionaryEnumerator source, System.Object target)
		{
			if((target.GetType())!= (typeof(System.Collections.IDictionaryEnumerator)))
				return false;
			else
				return EqualsSupport(source,(System.Collections.IDictionaryEnumerator)target);
		}

		/// <summary>
		/// Determines whether two IDictionaryEnumerator instances are equals.
		/// </summary>
		/// <param name="source">The first IDictionaryEnumerator to compare.</param>
		/// <param name="target">The second IDictionaryEnumerator to compare.</param>
		/// <returns>Return true if the first IDictionaryEnumerator contains the same values as the second IDictionaryEnumerator, otherwise return false.</returns>
		public static bool EqualsSupport(System.Collections.IDictionaryEnumerator source, System.Collections.IDictionaryEnumerator target )
		{
			while(source.MoveNext() && target.MoveNext())
				if (source.Key.Equals(target.Key))
					if(source.Value.Equals(target.Value))
						return true;
			return false;
		}

		/// <summary>
		/// Reverses the Stack Collection received.
		/// </summary>
		/// <param name="collection">The collection to reverse.</param>
		/// <returns>The collection received in reverse order if it was a System.Collections.Stack type, otherwise it does 
		/// nothing to the collection.</returns>
		public static System.Collections.IEnumerator ReverseStack(System.Collections.ICollection collection)
		{
			if((collection.GetType()) == (typeof(System.Collections.Stack)))
			{
				System.Collections.ArrayList collectionStack = new System.Collections.ArrayList(collection);
				collectionStack.Reverse();
				return collectionStack.GetEnumerator();
			}
			else
				return collection.GetEnumerator();
		}

		/*******************************/
		/// <summary>
		/// The class performs token processing from strings
		/// </summary>
		public class Tokenizer
		{
			//Element list identified
			private System.Collections.ArrayList elements;
			//Source string to use
			private string source;
			//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character
			private string delimiters = " \t\n\r";		

			/// <summary>
			/// Initializes a new class instance with a specified string to process
			/// </summary>
			/// <param name="source">String to tokenize</param>
			public Tokenizer(string source)
			{			
				this.elements = new System.Collections.ArrayList();
				this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				this.source = source;
			}

			/// <summary>
			/// Initializes a new class instance with a specified string to process
			/// and the specified token delimiters to use
			/// </summary>
			/// <param name="source">String to tokenize</param>
			/// <param name="delimiters">String containing the delimiters</param>
			public Tokenizer(string source, string delimiters)
			{
				this.elements = new System.Collections.ArrayList();
				this.delimiters = delimiters;
				this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				this.source = source;
			}

			/// <summary>
			/// Current token count for the source string
			/// </summary>
			public int Count
			{
				get
				{
					return (this.elements.Count);
				}
			}

			/// <summary>
			/// Determines if there are more tokens to return from the source string
			/// </summary>
			/// <returns>True or false, depending if there are more tokens</returns>
			public bool HasMoreTokens()
			{
				return (this.elements.Count > 0);			
			}

			/// <summary>
			/// Returns the next token from the token list
			/// </summary>
			/// <returns>The string value of the token</returns>
			public string NextToken()
			{			
				string result;
				if (source == "") throw new System.Exception();
				else
				{
					this.elements = new System.Collections.ArrayList();
					this.elements.AddRange(this.source.Split(delimiters.ToCharArray()));
					RemoveEmptyStrings();		
					result = (string) this.elements[0];
					this.elements.RemoveAt(0);				
					this.source = this.source.Remove(this.source.IndexOf(result),result.Length);
					this.source = this.source.TrimStart(this.delimiters.ToCharArray());
					return result;					
				}			
			}

			/// <summary>
			/// Returns the next token from the source string, using the provided
			/// token delimiters
			/// </summary>
			/// <param name="delimiters">String containing the delimiters to use</param>
			/// <returns>The string value of the token</returns>
			public string NextToken(string delimiters)
			{
				this.delimiters = delimiters;
				return NextToken();
			}

			/// <summary>
			/// Removes all empty strings from the token list
			/// </summary>
			private void RemoveEmptyStrings()
			{
				for (int index=0; index < this.elements.Count; index++)
					if ((string)this.elements[index]== "")
					{
						this.elements.RemoveAt(index);
						index--;
					}
			}
		}

		/*******************************/
		/// <summary>
		/// Creates an instance of a received Type.
		/// </summary>
		/// <param name="classType">The Type of the new class instance to return.</param>
		/// <returns>An Object containing the new instance.</returns>
		public static System.Object CreateNewInstance(System.Type classType)
		{
			System.Object instance = null;
			System.Type[] constructor = new System.Type[]{};
			System.Reflection.ConstructorInfo[] constructors = null;
       
			constructors = classType.GetConstructors();

			if (constructors.Length == 0)
				throw new System.UnauthorizedAccessException();
			else
			{
				for(int i = 0; i < constructors.Length; i++)
				{
					System.Reflection.ParameterInfo[] parameters = constructors[i].GetParameters();

					if (parameters.Length == 0)
					{
						instance = classType.GetConstructor(constructor).Invoke(new System.Object[]{});
						break;
					}
					else if (i == constructors.Length -1)     
						throw new System.MethodAccessException();
				}                       
			}
			return instance;
		}


		/*******************************/
		/// <summary>
		/// Writes the exception stack trace to the received stream
		/// </summary>
		/// <param name="throwable">Exception to obtain information from</param>
		/// <param name="stream">Output sream used to write to</param>
		public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream)
		{
			stream.Write(throwable.StackTrace);
			stream.Flush();
		}

		/*******************************/
		/// <summary>
		/// This class manages a set of elements.
		/// </summary>
		public class SetSupport : System.Collections.ArrayList
		{
			/// <summary>
			/// Creates a new set.
			/// </summary>
			public SetSupport(): base()
			{           
			}

			/// <summary>
			/// Creates a new set initialized with System.Collections.ICollection object
			/// </summary>
			/// <param name="collection">System.Collections.ICollection object to initialize the set object</param>
			public SetSupport(System.Collections.ICollection collection): base(collection)
			{           
			}

			/// <summary>
			/// Creates a new set initialized with a specific capacity.
			/// </summary>
			/// <param name="capacity">value to set the capacity of the set object</param>
			public SetSupport(int capacity): base(capacity)
			{           
			}
	 
			/// <summary>
			/// Adds an element to the set.
			/// </summary>
			/// <param name="objectToAdd">The object to be added.</param>
			/// <returns>True if the object was added, false otherwise.</returns>
			public new virtual bool Add(object objectToAdd)
			{
				if (this.Contains(objectToAdd))
					return false;
				else
				{
					base.Add(objectToAdd);
					return true;
				}
			}
	 
			/// <summary>
			/// Adds all the elements contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(System.Collections.ICollection collection)
			{
				bool result = false;
				if (collection!=null)
				{
					System.Collections.IEnumerator tempEnumerator = new System.Collections.ArrayList(collection).GetEnumerator();
					while (tempEnumerator.MoveNext())
					{
						if (tempEnumerator.Current != null)
							result = this.Add(tempEnumerator.Current);
					}
				}
				return result;
			}
		
			/// <summary>
			/// Adds all the elements contained in the specified support class collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(CollectionSupport collection)
			{
				return this.AddAll((System.Collections.ICollection)collection);
			}
	 
			/// <summary>
			/// Verifies that all the elements of the specified collection are contained into the current collection. 
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>True if the collection contains all the given elements.</returns>
			public virtual bool ContainsAll(System.Collections.ICollection collection)
			{
				bool result = false;
				System.Collections.IEnumerator tempEnumerator = collection.GetEnumerator();
				while (tempEnumerator.MoveNext())
					if (!(result = this.Contains(tempEnumerator.Current)))
						break;
				return result;
			}
		
			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(CollectionSupport collection)
			{
				return this.ContainsAll((System.Collections.ICollection) collection);
			}		
	 
			/// <summary>
			/// Verifies if the collection is empty.
			/// </summary>
			/// <returns>True if the collection is empty, false otherwise.</returns>
			public virtual bool IsEmpty()
			{
				return (this.Count == 0);
			}
	 	 
			/// <summary>
			/// Removes an element from the set.
			/// </summary>
			/// <param name="elementToRemove">The element to be removed.</param>
			/// <returns>True if the element was removed.</returns>
			public new virtual bool Remove(object elementToRemove)
			{
				bool result = false;
				if (this.Contains(elementToRemove))
					result = true;
				base.Remove(elementToRemove);
				return result;
			}
		
			/// <summary>
			/// Removes all the elements contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>True if all the elements were successfuly removed, false otherwise.</returns>
			public virtual bool RemoveAll(System.Collections.ICollection collection)
			{ 
				bool result = false;
				System.Collections.IEnumerator tempEnumerator = collection.GetEnumerator();
				while (tempEnumerator.MoveNext())
				{
					if ((result == false) && (this.Contains(tempEnumerator.Current)))
						result = true;
					this.Remove(tempEnumerator.Current);
				}
				return result;
			}
		
			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(CollectionSupport collection)
			{ 
				return this.RemoveAll((System.Collections.ICollection) collection);
			}		

			/// <summary>
			/// Removes all the elements that aren't contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>True if all the elements were successfully removed, false otherwise.</returns>
			public virtual bool RetainAll(System.Collections.ICollection collection)
			{
				bool result = false;
				System.Collections.IEnumerator tempEnumerator = collection.GetEnumerator();
				SetSupport tempSet = (SetSupport)collection;
				while (tempEnumerator.MoveNext())
					if (!tempSet.Contains(tempEnumerator.Current))
					{
						result = this.Remove(tempEnumerator.Current);
						tempEnumerator = this.GetEnumerator();
					}
				return result;
			}
		
			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(CollectionSupport collection)
			{
				return this.RetainAll((System.Collections.ICollection) collection);
			}		
	 
			/// <summary>
			/// Obtains an array containing all the elements of the collection.
			/// </summary>
			/// <returns>The array containing all the elements of the collection.</returns>
			public new virtual object[] ToArray()
			{
				int index = 0;
				object[] tempObject= new object[this.Count];
				System.Collections.IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					tempObject[index++] = tempEnumerator.Current;
				return tempObject;
			}

			/// <summary>
			/// Obtains an array containing all the elements in the collection.
			/// </summary>
			/// <param name="objects">The array into which the elements of the collection will be stored.</param>
			/// <returns>The array containing all the elements of the collection.</returns>
			public virtual object[] ToArray(object[] objects)
			{
				int index = 0;
				System.Collections.IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					objects[index++] = tempEnumerator.Current;
				return objects;
			}
		}
		/*******************************/
		/// <summary>
		/// Obtains a SetSupport instance with the entries contained in this IDictionary object.
		/// </summary>
		/// <param name="hashtable">The IDictionary of mappings.</param>
		/// <returns>Return a SetSupport object of the mappings contained in this IDictionary object.</returns>
		public static SetSupport EntrySet(System.Collections.IDictionary hashtable)
		{
			System.Collections.IDictionaryEnumerator hashEnumerator = hashtable.GetEnumerator();
			SetSupport hashSet = new SetSupport();
			while(hashEnumerator.MoveNext())
			{
				System.Collections.Hashtable tempHash = new System.Collections.Hashtable();
				tempHash.Add(hashEnumerator.Key, hashEnumerator.Value);
				hashSet.Add(tempHash.GetEnumerator());
			}
			return hashSet;
		}


	}
}
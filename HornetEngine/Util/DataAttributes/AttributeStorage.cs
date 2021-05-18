using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HornetEngine.Graphics
{
    /// <summary>
    /// Storage class for Attributes, stores attributes FiFo
    /// </summary>
    public class AttributeStorage : IEnumerable<Attribute>
    {
        private List<Attribute> attribs;
        private Mutex att_mutex;

        /// <summary>
        /// Creates a new instance of AttributeStorage with default parameters
        /// </summary>
        public AttributeStorage()
        {
            attribs = new List<Attribute>();
            att_mutex = new Mutex();
        }

        /// <summary>
        /// Retrieves an item on specified index from storage
        /// </summary>
        /// <param name="index">The index of the item in storage</param>
        /// <returns>Attribute on the index in the storage</returns>
        public Attribute this[int index] {
            get
            {
                return attribs[index];
            }
            set
            {
                attribs[index] = value;
            }
        }

        /// <summary>
        /// The amount of items inside the storage
        /// </summary>
        public int Count
        {
            get
            {
                return attribs.Count;
            }
        }

        /// <summary>
        /// Gets an attribute with specified name from the storage
        /// </summary>
        /// <param name="name">The name of the attribute that needs to be retrieved</param>
        /// <returns>Stored attribute if found, null if not found</returns>
        public Attribute GetAttribute(String name)
        {
            Attribute found = null;
            try
            {
                att_mutex.WaitOne();
                foreach (Attribute at in attribs)
                {
                    if (at.Name == name)
                    {
                        found = at;
                        break;
                    }
                }
            } catch(Exception ex)
            {
                throw ex;
            } finally
            {
                att_mutex.ReleaseMutex();
            }
            return found;
        }

        /// <summary>
        /// Adds an Attribute to the storage
        /// </summary>
        /// <param name="at">The Attribute to add to the storage</param>
        /// <returns>True if added, False if there was already an Attribute with that name</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool AddAttribute(Attribute at)
        {
            if(at == null)
            {
                throw new ArgumentNullException("Attribute cannot be null");
            }

            try
            {
                if (GetAttribute(at.Name) != null)
                {
                    return false;
                }
                att_mutex.WaitOne();
                attribs.Add(at);
                return true;
            } catch(Exception ex)
            {
                throw ex;
            } finally
            {
                att_mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Removes an Attribute from the storage
        /// </summary>
        /// <param name="name">The name of the Attribute that needs to be removed</param>
        /// <returns>True if an attribute was deleted, False if no attribute was deleted</returns>
        public bool RemoveAttribute(String name)
        {
            try
            {
                att_mutex.WaitOne();
                int del = attribs.RemoveAll((e) => { return e.Name == name; });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                att_mutex.ReleaseMutex();
            }
            return true;
        }

        /// <summary>
        /// Clears the Attribute storage
        /// </summary>
        public void ClearAttributes()
        {
            try
            {
                att_mutex.WaitOne();
                attribs.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                att_mutex.ReleaseMutex();
            }
        }

        public IEnumerator<Attribute> GetEnumerator()
        {
            return attribs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

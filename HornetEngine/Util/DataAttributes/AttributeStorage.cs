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
        /// Gets the total byte size of all the data that is stored in all attributes
        /// </summary>
        /// <returns>Total byte size of all data in attributes</returns>
        public int GetTotalByteCount()
        {
            int counter = 0;
            try
            {
                att_mutex.WaitOne();
                foreach(Attribute at in attribs)
                {
                    counter += at.byte_data.Count;
                }
                
            } catch(Exception)
            {
            
            } finally
            {
                att_mutex.ReleaseMutex();
            }
            return counter;
        }

        /// <summary>
        /// Checks if the data stored in each attribute has the same amount of datapoints
        /// </summary>
        /// <returns>True if the data is aligned, False if not</returns>
        public bool ValidateDataAlignment()
        {
            int prev_count = 0;
            int count = 0;
            try
            {
                att_mutex.WaitOne();
                if(attribs.Count == 0)
                {
                    return true;
                }
                Attribute at;
                for (int i = 0; i < attribs.Count; i++)
                {
                    at = attribs[i];
                    bool valid = at.ValidateDataIntegrity();
                    if (!valid)
                    {
                        throw new Exception("Attribute data in Attribute storage was not complete");
                    }

                    count = at.GetDatapointCount();
                    if(i != 0)
                    {
                        if (prev_count != count)
                        {
                            return false;
                        }
                    }
                    prev_count = count;
                }
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

        /// <summary>
        /// A function which getts the attribute's enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
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

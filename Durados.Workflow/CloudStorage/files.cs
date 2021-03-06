﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace Backand
{
    public class files : IFiles
    {
        //files.upload(parameters.filename, parameters.filedata, providerAccount,parameters.bucket, parameters.path );

        public string upload(string fileName, string fileData, string providerAccount, string bucket, string path)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Durados.DuradosException(Messages.MissingFileName);

            if (string.IsNullOrEmpty(fileData))
                throw new Durados.DuradosException(Messages.MissingFileData);

            IFiles files = StorageFactoey.GetCloudStorage(providerAccount);

            if (files == null || files.Cloud == null)
                return files.upload(fileName, fileData, null, path);

            if (string.IsNullOrEmpty(bucket))
                throw new Durados.DuradosException(Messages.MissingBucket);


            return files.upload(fileName, fileData, bucket, path);
        }

        public string upload(string fileName, string fileData, string accountProvider, string bucket)
        {
            return upload(fileName, fileData, accountProvider, bucket, null);
        }
        
        public string upload(string fileName, string fileData, string bucket)
        {
            return upload(fileName, fileData, null, bucket, null);
        }
        public string upload(string fileName, string fileData)
        {

           // IFiles files = StorageFactoey.GetCloudStorage(null);
            return new filesBackand(null).upload(fileName, fileData);
        }
       
        public void delete(string fileName, string providerAccount, string bucket,string path)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Durados.DuradosException(Messages.MissingFileName);

            if (string.IsNullOrEmpty(bucket))
                throw new Durados.DuradosException(Messages.MissingBucket);

            IFiles files = StorageFactoey.GetCloudStorage(providerAccount);
            files.delete(fileName, bucket,path);
            
        }

        public void delete(string fileName, string providerAccount, string bucket)
        {
            delete(fileName, providerAccount, bucket, null);
        }
        public void delete(string fileName, string bucket)
        {
            delete(fileName, null, bucket, null);
        }

        public void delete(string fileName)
        {
             new filesBackand(null).delete(fileName);
        }
        private Durados.Diagnostics.ILogger Logger
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    return null;
                return (Durados.Diagnostics.ILogger)System.Web.HttpContext.Current.Items[Durados.Database.MainLogger];
            }
        }

        public virtual Durados.Cloud Cloud
        {
            get { return null; }
        }
    }

    public interface IFiles
    {
        Durados.Cloud Cloud { get; }
        string upload(string fileName, string fileData, string providerAccount, string bucket);

        string upload(string fileName, string fileData, string providerAccount, string bucket, string path);

        string upload(string fileName, string fileData, string bucket);
        
        string upload(string fileName, string fileData);

        void delete(string fileName);

        void delete(string fileName, string bucket);
        void delete(string fileName, string bucket, string path);

    }

    public static class StorageFactoey
    {

        public static IFiles GetCloudStorage(string providerAccount = null)
        {
            //IFiles files = null;
         
            Durados.Cloud cloud = GetStorageCloud(providerAccount);
            if (cloud == null)
                return new filesBackand(null);

            return new cloudFiles(cloud);
           
        }

        private static Durados.Cloud GetStorageCloud(string providerAccount)
        {
            Dictionary<int, Durados.Cloud> storage = null;
            Durados.Cloud cloud = null;

            if (System.Web.HttpContext.Current.Items[Durados.Workflow.JavaScript.StorageAccountsKey] != null)
                storage = (System.Web.HttpContext.Current.Items[Durados.Workflow.JavaScript.StorageAccountsKey] as Dictionary<int, Durados.Cloud>);

            if (!string.IsNullOrEmpty(providerAccount) && storage != null && storage.Count>0) // case - the provider is provided
            {
                cloud = storage.Values.Where<Durados.Cloud>(v => v.Name.Equals(providerAccount, StringComparison.CurrentCulture)).FirstOrDefault();
                if(cloud == null)
                    throw new Durados.DuradosException(Messages.MissingStorageProvider);

            }

            else if (storage != null && storage.Count > 0)// case - the provider is NOT provided
            {
                cloud = storage.Values.FirstOrDefault();
            }
            
            if(!string.IsNullOrEmpty(providerAccount) && ( storage ==null ||storage.Count == 0))
                throw new   Durados.DuradosException(Messages.MissingStorageProvider);

            return cloud;
        }

         
    }

    public class Messages
    {
        public static readonly string MissingFileName = "Missing the fileName parameter.";
        public static readonly string MissingFileData = "Missing the filedata parameter.";
        public static readonly string MissingBucket = "Missing the bucket parameter.";
        public static string MissingStorageObjectInJS = "Missing storage object in ORM";
        public static string MissingStorageProvider = "Storage Provider account was not found.";

        
    }

    public class StorageKeys
    {
        public static readonly string Storage = "storage";
        public static readonly string FileName = "fileName";
        public static readonly string Filedata = "fileData";
        public static readonly string Credentials = "credentials";
    }
}

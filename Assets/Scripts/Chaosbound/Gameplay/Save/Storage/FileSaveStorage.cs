using System;
using System.IO;
using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    public sealed class FileSaveStorage : ISaveStorage
    {
        private readonly string filePath;
        private readonly string temporaryFilePath;

        public FileSaveStorage(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(
                    "Save file name cannot be null or empty.",
                    nameof(fileName));
            }

            fileName = fileName.Trim();

            if (Path.GetFileName(fileName) != fileName)
            {
                throw new ArgumentException(
                    "Save file name must not contain a directory path.",
                    nameof(fileName));
            }

            string directory =
                Application.persistentDataPath;

            filePath =
                Path.Combine(directory, fileName);

            temporaryFilePath =
                filePath + ".tmp";
        }

        public void Save(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            try
            {
                File.WriteAllText(
                    temporaryFilePath,
                    data);

                if (File.Exists(filePath))
                {
                    File.Replace(
                        temporaryFilePath,
                        filePath,
                        null);
                }
                else
                {
                    File.Move(
                        temporaryFilePath,
                        filePath);
                }
            }
            catch (Exception exception)
            {
                throw new IOException(
                    $"Failed to save data to '{filePath}'.",
                    exception);
            }
        }

        public bool TryLoad(out string data)
        {
            data = null;

            if (!File.Exists(filePath))
                return false;

            try
            {
                data = File.ReadAllText(filePath);
                return true;
            }
            catch (Exception exception)
            {
                throw new IOException(
                    $"Failed to load data from '{filePath}'.",
                    exception);
            }
        }

        public bool Exists()
        {
            return File.Exists(filePath);
        }

        public void Delete()
        {
            if (!File.Exists(filePath))
                return;

            try
            {
                File.Delete(filePath);
            }
            catch (Exception exception)
            {
                throw new IOException(
                    $"Failed to delete save file '{filePath}'.",
                    exception);
            }
        }
    }
}
﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using NUnit.Framework;
using SiliconStudio.Core.IO;

namespace SiliconStudio.Core.Design.Tests
{
    [TestFixture]
    public class TestUPath
    {
        [Test]
        public void TestNormalize()
        {
            string error;

            StringSpan driveSpan;
            StringSpan dirSpan;
            StringSpan nameSpan;

            var text = UPath.Normalize("test.txt", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("test.txt", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 8), dirSpan);

            text = UPath.Normalize("a", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);

            text = UPath.Normalize("a/b", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/b", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);
            Assert.AreEqual(new StringSpan(2, 1), nameSpan);

            // Test leading '..'
            text = UPath.Normalize("../a/b/c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("../a/b/c", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 6), dirSpan);
            Assert.AreEqual(new StringSpan(7, 1), nameSpan);

            // Test leading '..'
            text = UPath.Normalize("../a", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("../a", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 2), dirSpan);
            Assert.AreEqual(new StringSpan(3, 1), nameSpan);

            // Test leading '..'
            text = UPath.Normalize("../../a", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("../../a", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 5), dirSpan);
            Assert.AreEqual(new StringSpan(6, 1), nameSpan);

            // Test between '..'
            text = UPath.Normalize("a/../b/c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("b/c", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);
            Assert.AreEqual(new StringSpan(2, 1), nameSpan);

            // Test between '..'
            text = UPath.Normalize("a/b/../c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/c", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);
            Assert.AreEqual(new StringSpan(2, 1), nameSpan);

            // Test double '..'
            text = UPath.Normalize("a/../../c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("../c", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 2), dirSpan);
            Assert.AreEqual(new StringSpan(3, 1), nameSpan);

            // Test double '..' and trailing '..'
            text = UPath.Normalize("a/../../c/..", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("..", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 2), dirSpan);

            // Test double '..' and trailing '..'
            text = UPath.Normalize("a/../../c/../..", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("../..", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 5), dirSpan);

            // Test trailing '..'
            text = UPath.Normalize("a/b/c/..", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/b", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);
            Assert.AreEqual(new StringSpan(2, 1), nameSpan);

            // Test trailing '..' and trailing '/'
            text = UPath.Normalize("a/b/c/../", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/b", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);
            Assert.AreEqual(new StringSpan(2, 1), nameSpan);

            // Test root '.'
            text = UPath.Normalize(".", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual(".", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);

            // Test root '.'
            text = UPath.Normalize("././.", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual(".", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);

            // Test root '.'
            text = UPath.Normalize("a/././b", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/b", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);
            Assert.AreEqual(new StringSpan(2, 1), nameSpan);

            // Test leading '.'
            text = UPath.Normalize("././a/b", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/b", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);
            Assert.AreEqual(new StringSpan(2, 1), nameSpan);

            // Test trailing '.'
            text = UPath.Normalize("a/b/./.", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/b", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);
            Assert.AreEqual(new StringSpan(2, 1), nameSpan);

            // Test trailing '.'
            text = UPath.Normalize("a/b/././", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/b", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);
            Assert.AreEqual(new StringSpan(2, 1), nameSpan);
            
            // Test duplicate '/'
            text = UPath.Normalize("a////b/c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/b/c", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 3), dirSpan);
            Assert.AreEqual(new StringSpan(4, 1), nameSpan);

            // Test backslash '\'
            text = UPath.Normalize(@"\a\b\c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("/a/b/c", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 4), dirSpan);
            Assert.AreEqual(new StringSpan(5, 1), nameSpan);

            // Test leading multiple '/'
            text = UPath.Normalize("////a/b/c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("/a/b/c", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 4), dirSpan);
            Assert.AreEqual(new StringSpan(5, 1), nameSpan);

            // Test Trailing multiple '/'
            text = UPath.Normalize("a/b/c////", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("a/b/c", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 3), dirSpan);
            Assert.AreEqual(new StringSpan(4, 1), nameSpan);

            // Test multiple '/'
            text = UPath.Normalize("////", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("/", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 1), dirSpan);

            // Test rooted path '/a/b'
            text = UPath.Normalize("/a/b", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("/a/b", text.ToString());
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 2), dirSpan);
            Assert.AreEqual(new StringSpan(3, 1), nameSpan);

            // Test drive standard
            text = UPath.Normalize("C:a", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("C:a", text.ToString());
            Assert.IsTrue(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 2), driveSpan);
            Assert.AreEqual(new StringSpan(2, 1), dirSpan);

            // Test drive standard
            text = UPath.Normalize("C:/a/b/c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("C:/a/b/c", text.ToString());
            Assert.IsTrue(driveSpan.IsValid);
            Assert.IsTrue(dirSpan.IsValid);
            Assert.IsTrue(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 2), driveSpan);
            Assert.AreEqual(new StringSpan(2, 4), dirSpan);
            Assert.AreEqual(new StringSpan(7, 1), nameSpan);

            // Test drive backslash invalid
            UPath.Normalize("C:..", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNotNull(error);
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsFalse(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);

            // Test drive backslash invalid
            UPath.Normalize("C:/..", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNotNull(error);
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsFalse(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);

            // Test drive backslash invalid
            UPath.Normalize("C:/../", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNotNull(error);
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsFalse(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);

            // Test drive backslash invalid
            UPath.Normalize("C:/../..", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNotNull(error);
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsFalse(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);

            // Test drive backslash is valid
            text = UPath.Normalize("C:a/..", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNull(error);
            Assert.NotNull(text);
            Assert.AreEqual("C:", text.ToString());
            Assert.IsTrue(driveSpan.IsValid);
            Assert.IsFalse(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);
            Assert.AreEqual(new StringSpan(0, 2), driveSpan);

            // Test drive backslash is invalid
            UPath.Normalize("C:a/../..", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNotNull(error);
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsFalse(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);

            // Test drive start ':' is invali
            UPath.Normalize(":a/b/c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNotNull(error);
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsFalse(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);

            // Test drive in the middle ':' is invalid
            UPath.Normalize("a/c:a/b/c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNotNull(error);
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsFalse(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);

            // Test drive multiple ':' is invalid
            UPath.Normalize("a:c:a/b/c", out driveSpan, out dirSpan, out nameSpan, out error);
            Assert.IsNotNull(error);
            Assert.IsFalse(driveSpan.IsValid);
            Assert.IsFalse(dirSpan.IsValid);
            Assert.IsFalse(nameSpan.IsValid);
        }

        [Test]
        public void TestIsAbsolute()
        {
            Assert.True(new UFile("/a/b/c.txt").IsAbsolute);
            Assert.True(new UFile("E:/a/b/c.txt").IsAbsolute);
            Assert.True(new UDirectory("/c.txt").IsAbsolute);
            Assert.True(new UDirectory("/").IsAbsolute);
            Assert.False(new UFile("a/b/c.txt").IsAbsolute);
            Assert.False(new UFile("../c.txt").IsAbsolute);
        }

        [Test]
        public void TestFileExtension()
        {
            Assert.AreEqual("test", new UFile("test.txt").GetFileName());
            Assert.AreEqual(".txt", new UFile("test.txt").GetFileExtension());

            Assert.AreEqual("test.another", new UFile("test.another.txt").GetFileName());
            Assert.AreEqual(".txt", new UFile("test.another.txt").GetFileExtension());

            Assert.AreEqual(".txt", new UFile(".txt").GetFileExtension());
            Assert.IsFalse(new UFile("test.txt").IsAbsolute);
        }

        [Test]
        public void TestIsDirectoryOnly()
        {
            Assert.False(new UFile("/a/b/c.txt").IsDirectoryOnly);

            var dirPath = new UDirectory("/a/b/c");
            Assert.True(dirPath.IsDirectoryOnly);
            Assert.AreEqual("/a/b/c", dirPath.GetDirectory());

            var filePath = new UFile("/test.txt");
            Assert.AreEqual("/", filePath.GetDirectory());
            Assert.AreEqual("test.txt", filePath.GetFileNameWithExtension());
        }

        [Test, Ignore]
        public void TestWithSimpleDirectory()
        {
            var assetPath = new UDirectory("/a/b/c");
            Assert.AreEqual("/a/b/c", assetPath.GetDirectory());
            Assert.AreEqual("/a/b/c", assetPath.FullPath);
            var directory = new UDirectory("/a");
            Assert.AreEqual("/a", directory.GetDirectory());
        }

        [Test]
        public void TestWithSimplePath()
        {
            var assetPath = new UFile("/a/b/c");
            Assert.AreEqual("/a/b", assetPath.GetDirectory());
            Assert.AreEqual("c", assetPath.GetFileName());
            Assert.AreEqual(null, assetPath.GetFileExtension());
            Assert.AreEqual("/a/b/c", assetPath.GetDirectoryAndFileName());
            Assert.AreEqual("/a/b/c", assetPath.FullPath);
        }

        [Test]
        public void TestWithSimplePathWithExtension()
        {
            var assetPath = new UFile("/a/b/c.txt");
            Assert.AreEqual("/a/b", assetPath.GetDirectory());
            Assert.AreEqual("c", assetPath.GetFileName());
            Assert.AreEqual(".txt", assetPath.GetFileExtension());
            Assert.AreEqual("/a/b/c", assetPath.GetDirectoryAndFileName());
            Assert.AreEqual("/a/b/c.txt", assetPath.FullPath);
        }

        [Test]
        public void TestWithNormalization()
        {
            var assetPath = new UFile("/a/b/.././././//c.txt");
            Assert.AreEqual("/a", assetPath.GetDirectory());
            Assert.AreEqual("c", assetPath.GetFileName());
            Assert.AreEqual(".txt", assetPath.GetFileExtension());
            Assert.AreEqual("/a/c", assetPath.GetDirectoryAndFileName());
            Assert.AreEqual("/a/c.txt", assetPath.FullPath);

            assetPath = new UFile("../.././././//c.txt");
            Assert.AreEqual("../..", assetPath.GetDirectory());
            Assert.AreEqual("c", assetPath.GetFileName());
            Assert.AreEqual(".txt", assetPath.GetFileExtension());
            Assert.AreEqual("../../c", assetPath.GetDirectoryAndFileName());
            Assert.AreEqual("../../c.txt", assetPath.FullPath);

            assetPath = new UFile("a/../../../c.txt");
            Assert.AreEqual("../../c.txt", assetPath.FullPath);
        }

        [Test]
        public void TestEquals()
        {
            var assetPath1 = new UFile(null);
            var assetPath2 = new UFile(null);
            Assert.AreEqual(assetPath1, assetPath2);

            assetPath1 = new UFile("/a/b/c.txt");
            assetPath2 = new UFile("/a/b/d/../c.txt");
            Assert.AreEqual(assetPath1, assetPath2);

            // Test is not done on Extensions
            assetPath1 = new UFile("/a/b/c.txt");
            assetPath2 = new UFile("/a/b/d/../c.png");
            Assert.AreNotEqual(assetPath1, assetPath2);
            Assert.AreEqual(assetPath1.GetDirectoryAndFileName(), assetPath2.GetDirectoryAndFileName());
        }

        [Test]
        public void TestCombine()
        {
            var path = UPath.Combine("/a/b/c", new UFile("../d/e.txt"));
            Assert.AreEqual("/a/b/d/e.txt", path.ToString());
        }

        [Test]
        public void TestMixedSlash()
        {
            var assetPath1 = new UFile("/a\\b/c\\d.txt");
            var assetPath2 = new UFile("/a/b/c/d.txt");
            Assert.AreEqual(assetPath1.ToString(), assetPath2.ToString());
        }

        [Test]
        public void TestGetParent()
        {
            var dirPath1 = new UDirectory(@"E:\a\b");

            var dirPathParent1 = dirPath1.GetParent();
            Assert.AreEqual(@"E:/a", dirPathParent1.FullPath);

            dirPathParent1 = dirPathParent1.GetParent();
            Assert.AreEqual(@"E:", dirPathParent1.FullPath);

            dirPathParent1 = dirPathParent1.GetParent();
            Assert.AreEqual(UDirectory.Empty, dirPathParent1);

            dirPath1 = new UDirectory(@"/a/b");

            dirPathParent1 = dirPath1.GetParent();
            Assert.AreEqual(@"/a", dirPathParent1.FullPath);

            dirPathParent1 = dirPathParent1.GetParent();
            Assert.AreEqual(@"/", dirPathParent1.FullPath);

            dirPathParent1 = dirPathParent1.GetParent();
            Assert.AreEqual(UDirectory.Empty, dirPathParent1);

            // Test on file
            var filePath1 = new UFile(@"/a/b.txt");

            dirPathParent1 = filePath1.GetParent();
            Assert.AreEqual(@"/a", dirPathParent1.FullPath);
        }

        [Test]
        public void TestMakeRelative()
        {
            UPath assetPath2 = null;
            UPath newAssetPath2 = null;
            var dir1 = new UDirectory("/a/b/c");

            var assetDir2 = new UDirectory("/a/b/c");
            newAssetPath2 = dir1.MakeRelative(assetDir2);
            Assert.AreEqual(".", newAssetPath2.FullPath);

            var assetDir3 = new UDirectory("/a/b");
            newAssetPath2 = dir1.MakeRelative(assetDir3);
            Assert.AreEqual("c", newAssetPath2.FullPath);

            var assetDir4 = new UDirectory("/a/b/c/d");
            newAssetPath2 = dir1.MakeRelative(assetDir4);
            Assert.AreEqual("..", newAssetPath2.FullPath);

            // Test direct relative
            assetPath2 = new UFile("/a/b/c/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("test.txt", newAssetPath2.FullPath);

            // Test direct relative + subdir
            assetPath2 = new UFile("/a/b/c/test/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("test/test.txt", newAssetPath2.FullPath);

            // Test relative 1
            assetPath2 = new UFile("/a/b/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../test.txt", newAssetPath2.FullPath);

            // Test relative 2
            assetPath2 = new UFile("/a/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../../test.txt", newAssetPath2.FullPath);

            // Test relative 3
            assetPath2 = new UFile("/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../../../test.txt", newAssetPath2.FullPath);

            // Test already relative
            assetPath2 = new UFile("../test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../test.txt", newAssetPath2.FullPath);

            // Test only root path in common
            assetPath2 = new UFile("/e/f/g/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../../../e/f/g/test.txt", newAssetPath2.FullPath);

            // Test only root path in common with single file
            assetPath2 = new UFile("/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../../../test.txt", newAssetPath2.FullPath);
        }

        [Test]
        public void TestMakeRelativeWithDrive()
        {
            UPath assetPath2 = null;
            UPath newAssetPath2 = null;
            var dir1 = new UDirectory("C:/a/b/c");

            // Test direct relative
            assetPath2 = new UFile("C:/a/b/c/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("test.txt", newAssetPath2.FullPath);

            // Test direct relative + subdir
            assetPath2 = new UFile("C:/a/b/c/test/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("test/test.txt", newAssetPath2.FullPath);

            // Test relative 1
            assetPath2 = new UFile("C:/a/b/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../test.txt", newAssetPath2.FullPath);

            // Test relative 2
            assetPath2 = new UFile("C:/a/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../../test.txt", newAssetPath2.FullPath);

            // Test relative 3
            assetPath2 = new UFile("C:/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../../../test.txt", newAssetPath2.FullPath);

            // Test already relative
            assetPath2 = new UFile("../test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("../test.txt", newAssetPath2.FullPath);

            // Test no path in common
            assetPath2 = new UFile("E:/e/f/g/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("E:/e/f/g/test.txt", newAssetPath2.FullPath);

            // Test no root path single file
            assetPath2 = new UFile("E:/test.txt");
            newAssetPath2 = assetPath2.MakeRelative(dir1);
            Assert.AreEqual("E:/test.txt", newAssetPath2.FullPath);
        }

        [Test]
        public void TestContains()
        {
            var dir1 = new UDirectory("C:/a/b/c");
            Assert.IsTrue(dir1.Contains(new UFile("C:/a/b/c/d")));
            Assert.IsTrue(dir1.Contains(new UFile("C:/a/b/c/d/e")));
            Assert.IsTrue(dir1.Contains(new UDirectory("C:/a/b/c/d")));
            Assert.IsTrue(dir1.Contains(new UDirectory("C:/a/b/c/d/e")));
            Assert.IsFalse(dir1.Contains(new UFile("C:/a/b/x")));
            Assert.IsFalse(dir1.Contains(new UFile("C:/a/b/cx")));
        }
    }
}
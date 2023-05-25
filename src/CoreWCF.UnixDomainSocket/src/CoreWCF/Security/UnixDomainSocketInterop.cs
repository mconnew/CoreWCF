﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace CoreWCF.UnixDomainSocket.Security
{
    internal static class UnixDomainSocketInterop
    {
        [SupportedOSPlatform("linux")]
        internal static bool TryGetCredentials(this Socket socket, out uint processId, out uint userId, out uint groupId)
        {
            if (!OperatingSystem.IsLinux())
                throw new PlatformNotSupportedException();

            const int SOL_SOCKET = 1;
            const int SO_PEERCRED = 17;

            Span<uint> ucred = stackalloc uint[3];
            var bytesWritten = socket.GetRawSocketOption(SOL_SOCKET, SO_PEERCRED, MemoryMarshal.AsBytes(ucred));
            processId = ucred[0];
            userId = ucred[1];
            groupId = ucred[2];
            return bytesWritten == (ucred.Length * sizeof(uint));
        }

    }

    internal class NativeSysCall
    {
        const string LIBC = "libc";

        [DllImport(LIBC, SetLastError = true)]
        private static extern IntPtr getgrgid(uint name);

        [DllImport(LIBC, SetLastError = true)]
        private static extern IntPtr getpwuid(uint uid);

        internal static UserInfo? GetUserInfo(uint uid)
        {
            var result = NativeSysCall.getpwuid(uid);
            if (result != IntPtr.Zero) return new UserInfo(Marshal.PtrToStructure<NativeSysCall.Passwd>(result));
            return null;
        }

        internal static GroupInfo? GetGroupInfo(uint grpId)
        {
            var result = NativeSysCall.getgrgid(grpId);
            if (result != IntPtr.Zero) return new GroupInfo(Marshal.PtrToStructure<NativeSysCall.Group>(result));
            return null;
        }

        internal struct Group
        {
            public string Name;
            public string Password;
            public uint Gid;
            public IntPtr Members;

        }

        internal struct Passwd
        {
            public string Name;
            public string Password;
            public uint Uid;
            public uint Gid;
            public string GECOS;
            public string Directory;
            public string Shell;

        }

    }
    internal sealed class GroupInfo
    {

        public string Name { get; }
        public uint Id { get; }
        public string[] Members { get; }

        internal GroupInfo(NativeSysCall.Group group)
        {
            Name = group.Name;
            Id = group.Gid;
            Members = GetMembers(group.Members).ToArray();
        }

        private static IEnumerable<string> GetMembers(IntPtr members)
        {
            IntPtr p;
            for (int i = 0; (p = Marshal.ReadIntPtr(members, i * IntPtr.Size)) != IntPtr.Zero; i++)
                yield return Marshal.PtrToStringAnsi(p)!;
        }

    }

    internal class UserInfo
    {
        public string Name { get; }
        public uint Uid { get; }
        public uint Gid { get; }
        public string? Directory { get; }
        public string? Shell { get; }

        internal UserInfo(NativeSysCall.Passwd passwd)
        {
            Name = passwd.Name;
            Uid = passwd.Uid;
            Gid = passwd.Gid;
            Directory = passwd.Directory;
            Shell = passwd.Shell;
        }

    }
}


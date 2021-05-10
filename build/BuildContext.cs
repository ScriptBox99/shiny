﻿using System;
using System.IO;
using Cake.Common;
using Cake.Common.Build;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.Git;


namespace ShinyBuild
{
    public class BuildContext : FrostingContext
    {
        public BuildContext(ICakeContext context) : base(context)
        {
            this.MsBuildConfiguration = context.Argument("configuration", "Release");
            this.NugetApiKey = context.EnvironmentVariable("NugetApiKey");

            // walk backwards until git directory found - that's root
            var dir = new DirectoryPath(".");
            while (!context.GitIsValidRepository(dir))
                dir = new DirectoryPath(Directory.GetParent(dir.FullPath).FullName);

            context.Environment.WorkingDirectory = dir;
            this.Branch = context.GitBranchCurrent(".");

            this.GitVersionLog = new FilePath("./gitversion.log");
        }


        public string MsBuildConfiguration { get; }
        public string NugetApiKey { get; }
        public FilePath GitVersionLog { get; }
        public GitBranch Branch { get; }
        public bool IsMainBranch
        {
            get
            {
                var bn = this.Branch.FriendlyName.ToLower();
                return bn.Equals("main") || bn.Equals("master");
            }
        }


        public bool IsRunningInCI
        {
            get
            {
                var ga = this.GitHubActions();
                if (!ga.IsRunningOnGitHubActions)
                    return false;

                //if (ga.Environment.PullRequest.IsPullRequest)
                return true;
            }
        }
    }
}
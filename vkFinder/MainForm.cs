﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using VkNet;
using VkNet.Categories;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace vkFinder
{
    public partial class MainForm : Form
    {
        public static VkApi Vk = new VkApi();

        public MainForm()
        {
            InitializeComponent();
        }

        public void Authorize(string login, string password)
        {
            var scope = Settings.All;
            var appId = AppSettings.Default.app_id;
            Vk.Authorize(new ApiAuthParams
            {
                ApplicationId = appId,
                Login = login,
                Password = password,
                Settings = scope
            });
        }

        public void SetUpUi()
        {
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            userListView.Items.Clear();
            userProcesser.RunWorkerAsync();
            startButton.Enabled = false;
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (userListView.SelectedItems.Count == 0) return;
            userName.Text = userListView.SelectedItems[0].SubItems[0].Text;
            userProfileUrl.Text = userListView.SelectedItems[0].SubItems[1].Text;
            userPhoto.ImageLocation = userListView.SelectedItems[0].SubItems[2].Text;
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Vk.IsAuthorized) return;
            var profileinfo = Vk.Account.GetProfileInfo();
            var groups = GetSelfGroups();
            if (selfName.InvokeRequired)
                selfName.Invoke((Action) (() => selfName.Text = $@"{profileinfo.FirstName} {profileinfo.LastName}"));
            if (selfGroups.InvokeRequired)
                selfGroups.Invoke((Action) (() => selfGroups.Text = $@"Количество групп: {groups.Count}"));
            var utils = Vk.Utils;
            var searchGroup = ResolveGroup(utils);
            var min = (ushort) minAge.Value;
            var max = (ushort) maxAge.Value;
            var i = 0;
            int matches = Convert.ToInt16(minIntGroups.Value);
            var sex = sexMale.Checked ? Sex.Male : Sex.Female;
            if (searchGroup.Id == null) return;
            var users = UsersSearch(searchGroup, min, max, sex);
            if (usersCount.InvokeRequired)
                usersCount.Invoke((Action) (() => usersCount.Text = $@"Найдено: {users.Count}"));
            if (userCheckingProgress.InvokeRequired)
                userCheckingProgress.Invoke((Action) (() => userCheckingProgress.Maximum = users.Count));

            foreach (var user in users)
                try
                {
                    var userGroups = GetUserGroups(user);
                    var userFirstName = user.FirstName;
                    var userLastName = user.LastName;
                    var photoLink = user.Photo100.AbsoluteUri;
                    var userLink = "https://vk.com/id" + user.Id;
                    var both = userGroups.Intersect(groups, new GroupComparer()).ToList();
                    var intersections = both.Count;
                    if (intersections >= matches)
                    {
                        string[] userInfo = {$"{userFirstName} {userLastName}", userLink, photoLink};
                        var listViewItem = new ListViewItem(userInfo);
                        if (userListView.InvokeRequired)
                            userListView.Invoke((Action) (() => userListView.Items.Add(listViewItem)));
                    }
                    i += 1;
                    userProcesser.ReportProgress(i);
                }
                catch (AccessDeniedException)
                {
                    // ignored
                }
        }

        private static IEnumerable<Group> GetUserGroups(User user)
        {
            return Vk.Groups.Get(new GroupsGetParams
            {
                Count = 1000,
                UserId = user.Id
            }).ToList();
        }

        private VkObject ResolveGroup(UtilsCategory utils)
        {
            return utils.ResolveScreenName(groupScreenName.Text);
        }

        private static List<Group> GetSelfGroups()
        {
            return Vk.Groups.Get(new GroupsGetParams
            {
                UserId = Vk.UserId,
                Count = 1000
            }).ToList();
        }

        private static List<User> UsersSearch(VkObject searchGroup, ushort min, ushort max, Sex sex)
        {
            return Vk.Users.Search(new UserSearchParams
            {
                AgeFrom = min,
                AgeTo = max,
                Sort = 0,
                Count = 1000,
                Country = 1,
                Hometown = "Хабаровск",
                Sex = sex,
                Fields = ProfileFields.Photo100,
                // ReSharper disable once PossibleInvalidOperationException
                GroupId = (ulong) searchGroup.Id
            }).ToList();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            userCheckingProgress.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            userCheckingProgress.Value = userCheckingProgress.Maximum;
            startButton.Enabled = true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var settingsForm = new Form2();
            settingsForm.Show();
            authorization.Enabled = false;
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            userProfileUrl.Links[userProfileUrl.Links.IndexOf(e.Link)].Visited = true;
            Process.Start(userProfileUrl.Text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = $@"vkGirlFinder v{AppSettings.Default.version}";
        }

        private class GroupComparer : IEqualityComparer<Group>
        {
            public bool Equals(Group x, Group y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(Group group)
            {
                if (ReferenceEquals(group, null))
                    return 0;

                var hashProductName = group.Name == null ? 0 : group.Name.GetHashCode();
                var hashProductCode = group.Id.GetHashCode();

                return hashProductName ^ hashProductCode;
            }
        }
    }
}
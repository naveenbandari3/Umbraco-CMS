﻿
using NUnit.Framework;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;

namespace Umbraco.Cms.Tests.Integration.Umbraco.Infrastructure.Services;

public partial class UserServiceCrudTests
{
    [Test]
    public async Task Only_Super_User_Can_Get_Super_user()
    {
        var userService = CreateUserService();
        var editorGroup = await UserGroupService.GetAsync(Constants.Security.EditorGroupAlias);

        var nonSuperCreateModel = new UserCreateModel
        {
            Email = "not@super.com",
            UserName = "not@super.com",
            UserGroups = new HashSet<IUserGroup> { editorGroup! },
            Name = "Not A Super User"
        };

        var createEditorAttempt = await userService.CreateAsync(Constants.Security.SuperUserKey, nonSuperCreateModel, true);
        Assert.IsTrue(createEditorAttempt.Success);

        var editor = createEditorAttempt.Result.CreatedUser;
        var allUsersAttempt = await userService.GetAllAsync(editor!.Key, 0, 10000);

        Assert.IsTrue(allUsersAttempt.Success);
        var result = allUsersAttempt.Result;
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count());
        Assert.AreEqual(1, result.Total);
        var onlyUser = result.Items.First();
        Assert.AreEqual(editor.Key, onlyUser.Key);
    }

    [Test]
    public async Task Super_User_Can_See_Super_User()
    {
        var userService = CreateUserService();
        var editorGroup = await UserGroupService.GetAsync(Constants.Security.EditorGroupAlias);

        var nonSuperCreateModel = new UserCreateModel
        {
            Email = "not@super.com",
            UserName = "not@super.com",
            UserGroups = new HashSet<IUserGroup> { editorGroup! },
            Name = "Not A Super User"
        };

        var createEditorAttempt = await userService.CreateAsync(Constants.Security.SuperUserKey, nonSuperCreateModel, true);
        Assert.IsTrue(createEditorAttempt.Success);

        var editor = createEditorAttempt.Result.CreatedUser;
        var allUsersAttempt = await userService.GetAllAsync(Constants.Security.SuperUserKey, 0, 10000);
        Assert.IsTrue(allUsersAttempt.Success);
        var result = allUsersAttempt.Result;

        Assert.AreEqual(2, result.Items.Count());
        Assert.AreEqual(2, result.Total);
        Assert.IsTrue(result.Items.Any(x => x.Key == Constants.Security.SuperUserKey));
        Assert.IsTrue(result.Items.Any(x => x.Key == editor!.Key));
    }

    [Test]
    public async Task Only_Admins_Can_See_Admins()
    {
        var userService = CreateUserService();
        var adminGroup = await UserGroupService.GetAsync(Constants.Security.AdminGroupAlias);
        var editorGroup = await UserGroupService.GetAsync(Constants.Security.EditorGroupAlias);

        var editorCreateModel = new UserCreateModel
        {
            UserName = "editor@mail.com",
            Email = "editor@mail.com",
            Name = "Editor Mc. Gee",
            UserGroups = new HashSet<IUserGroup> { editorGroup! }
        };

        var adminCreateModel = new UserCreateModel
        {
            UserName = "admin@mail.com",
            Email = "admin@mail.com",
            Name = "Admin Mc. Gee",
            UserGroups = new HashSet<IUserGroup> { adminGroup!, editorGroup }
        };

        var createEditorAttempt = await userService.CreateAsync(Constants.Security.SuperUserKey, editorCreateModel, true);
        var createAdminAttempt = await userService.CreateAsync(Constants.Security.SuperUserKey, adminCreateModel, true);

        Assert.IsTrue(createEditorAttempt.Success);
        Assert.IsTrue(createAdminAttempt.Success);

        var editorAllUsersAttempt = await userService.GetAllAsync(createEditorAttempt.Result.CreatedUser!.Key, 0, 10000);
        Assert.IsTrue(editorAllUsersAttempt.Success);
        var editorAllUsers = editorAllUsersAttempt.Result.Items.ToList();
        Assert.AreEqual(1, editorAllUsers.Count);
        Assert.AreEqual(createEditorAttempt.Result.CreatedUser!.Key, editorAllUsers.First().Key);

        var adminAllUsersAttempt = await userService.GetAllAsync(createAdminAttempt.Result.CreatedUser!.Key, 0, 10000);
        Assert.IsTrue(adminAllUsersAttempt.Success);
        var adminAllUsers = adminAllUsersAttempt.Result.Items.ToList();
        Assert.AreEqual(2, adminAllUsers.Count);
        Assert.IsTrue(adminAllUsers.Any(x => x.Key == createEditorAttempt.Result.CreatedUser!.Key));
        Assert.IsTrue(adminAllUsers.Any(x => x.Key == createAdminAttempt.Result.CreatedUser!.Key));
    }
}

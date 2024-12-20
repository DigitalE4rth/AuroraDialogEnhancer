﻿using System;
using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

namespace AuroraDialogEnhancer.AppConfig.Updater;

public class PatchService
{
    private readonly ExtensionsProvider _extensionsProvider;
    private readonly KeyBindingProfileService _keyBindingProfileService;
    
    private readonly List<(Version, Action)> _patchList;

    public PatchService(ExtensionsProvider extensionsProvider, KeyBindingProfileService keyBindingProfileService)
    {
        _extensionsProvider       = extensionsProvider;
        _keyBindingProfileService = keyBindingProfileService;

        _patchList = new List<(Version, Action)>
        {
            new(new Version(1, 0, 5, 0), UpdateMissingKeys1050)
        };
    }

    public bool Patch(Version previous)
    {
        previous = new Version(Properties.Settings.Default.App_CurrentVersion);
        var current = AppConstants.AssemblyInfo.Version;
        if (previous >= current) return false;

        var patches = 
            _patchList.Where(tuple => tuple.Item1.Major <= current.Major
                                   && tuple.Item1.Major >= previous.Major
                                   && tuple.Item1.Minor <= current.Minor
                                   && tuple.Item1.Minor >= previous.Minor
                                   && tuple.Item1.Build <= current.Build
                                   && tuple.Item1.Build >= previous.Build
                                   && tuple.Item1.Revision <= current.Revision
                                   && tuple.Item1.Revision >= previous.Revision)
                .OrderBy(tuple => tuple.Item1)
                .ToList();

        if (!patches.Any()) return false;
        
        patches.ForEach(tuple => tuple.Item2.Invoke());
        return true;
    }

    public void UpdateMissingKeys1050()
    {
        foreach (var id in _extensionsProvider.ExtensionsDictionary.Keys)
        {
            var userProfile = _keyBindingProfileService.Get(id);
            var defaultProfile = _keyBindingProfileService.GetDefault(id);

            var allKeys = _keyBindingProfileService.GetAllKeys(userProfile);

            #region Last Dialog Option
            if (!userProfile.Last.Any())
            {
                AddMissingUnusedKeys(defaultProfile.Last, userProfile.Last, allKeys);
            }
            #endregion

            #region AutoSkip
            if (!userProfile.AutoSkipConfig.ActivationKeys.Any())
            {
                AddMissingUnusedKeys(defaultProfile.AutoSkipConfig.ActivationKeys, userProfile.AutoSkipConfig.ActivationKeys, allKeys);
            }

            if (!userProfile.AutoSkipConfig.SkipKeys.Any())
            {
                userProfile.AutoSkipConfig.SkipKeys.AddRange(defaultProfile.AutoSkipConfig.SkipKeys);
                allKeys.Add(defaultProfile.AutoSkipConfig.SkipKeys);
            }

            if (userProfile.AutoSkipConfig.ClickDelayRegular == 0)
            {
                userProfile.AutoSkipConfig.ClickDelayRegular = defaultProfile.AutoSkipConfig.ClickDelayRegular;
            }

            if (userProfile.AutoSkipConfig.ScanDelayRegular == 0)
            {
                userProfile.AutoSkipConfig.ScanDelayRegular = defaultProfile.AutoSkipConfig.ScanDelayRegular;
            }

            if (userProfile.AutoSkipConfig.ScanDelayReply == 0)
            {
                userProfile.AutoSkipConfig.ScanDelayReply = defaultProfile.AutoSkipConfig.ScanDelayReply;
            }

            if (userProfile.AutoSkipConfig.ClickDelayReply == 0)
            {
                userProfile.AutoSkipConfig.ClickDelayReply = defaultProfile.AutoSkipConfig.ClickDelayReply;
            }
            #endregion

            #region InteractionPoints
            foreach (var defaultPoint in from dip in defaultProfile.InteractionPoints
                     let uip = userProfile.InteractionPoints.FirstOrDefault(ip => ip.Id.Equals(dip.Id))
                     where uip is null
                     select dip)
            {
                userProfile.InteractionPoints.Add(defaultPoint);

                foreach (var defaultPointKeys in defaultPoint.ActivationKeys.ToList())
                {
                    if (_keyBindingProfileService.AreKeysAlreadyInUse(allKeys, defaultPointKeys))
                    {
                        userProfile.InteractionPoints.Last().ActivationKeys.Remove(defaultPointKeys);
                    }

                    allKeys.Add(defaultPointKeys);
                }
            }
            #endregion

            _keyBindingProfileService.Save(id, userProfile);
        }
    }

    private void AddMissingUnusedKeys(List<List<GenericKey>> defaultProfileKeys, List<List<GenericKey>> userProfileKeys, List<List<GenericKey>> allKeys)
    {
        foreach (var autoSkipTrigger in defaultProfileKeys.Where(autoSkipTrigger =>
                     !_keyBindingProfileService.AreKeysAlreadyInUse(allKeys, autoSkipTrigger)))
        {
            userProfileKeys.Add(autoSkipTrigger);
            allKeys.Add(autoSkipTrigger);
        }
    }
}

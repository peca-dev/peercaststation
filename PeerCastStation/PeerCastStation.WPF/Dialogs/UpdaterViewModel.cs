﻿// PeerCastStation, a P2P streaming servent.
// Copyright (C) 2013 PROGRE (djyayutto@gmail.com)
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using PeerCastStation.UI;
using PeerCastStation.WPF.Commons;

namespace PeerCastStation.WPF.Dialogs
{
  class UpdaterViewModel:ViewModelBase
  {
    private readonly IEnumerable<VersionDescription> versionInfo;

    private readonly Command download;
    public Command Download { get { return download; } }

    public string Descriptions
    {
      get {
        return String.Join("\n", versionInfo.Select(v => v.Description).ToArray());
      }
    }

    public UpdaterViewModel(IEnumerable<VersionDescription> versionInfo)
    {
      this.versionInfo = versionInfo;

      download = new Command(
        () => Process.Start(versionInfo.First().Link.ToString()));
    }
  }
}

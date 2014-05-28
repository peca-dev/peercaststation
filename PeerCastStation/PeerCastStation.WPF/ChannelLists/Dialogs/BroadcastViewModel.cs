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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PeerCastStation.Core;
using PeerCastStation.WPF.ChannelLists.ChannelInfos;
using PeerCastStation.WPF.Commons;
using PeerCastStation.WPF.CoreSettings;

namespace PeerCastStation.WPF.ChannelLists.Dialogs
{
  class BroadcastViewModel : ViewModelBase
  {
    private readonly IContentReaderFactory[] contentTypes;
    public IContentReaderFactory[] ContentTypes { get { return contentTypes; } }

    private readonly IEnumerable<KeyValuePair<string,IYellowPageClient>> yellowPages;
    public IEnumerable<KeyValuePair<string,IYellowPageClient>> YellowPages { get { return yellowPages; } }

    private UISettingsViewModel uiSettings;
    public IEnumerable<BroadcastInfoViewModel> BroadcastHistory {
      get { return uiSettings.BroadcastHistory.OrderBy(i => i.Favorite ? 0 : 1); }
    }

    private BroadcastInfoViewModel selectedBroadcastHistory;
    public BroadcastInfoViewModel SelectedBroadcastHistory {
      get { return selectedBroadcastHistory; }
      set {
        if (value!=null) {
          if (value.StreamType!=null) {
            SelectedSourceStream = SourceStreams.FirstOrDefault(t => t.Name==value.StreamType);
          }
          ContentType = contentTypes.FirstOrDefault(t => t.Name==value.ContentType);
          StreamUrl   = value.StreamUrl;
          Bitrate     = value.Bitrate==0 ? null : value.Bitrate.ToString();
          ContentType = contentTypes.FirstOrDefault(t => t.Name==value.ContentType);
          if (value.YellowPage!=null) {
            YellowPage = yellowPages.Where(y => y.Value!=null).FirstOrDefault(y => y.Value.Name==value.YellowPage).Value;
          }
          else {
            YellowPage = null;
          }
          ChannelName = value.ChannelName;
          Genre       = value.Genre;
          Description = value.Description;
          Comment     = value.Comment;
          ContactUrl  = value.ContactUrl;
          TrackTitle  = value.TrackTitle;
          TrackAlbum  = value.TrackAlbum;
          TrackArtist = value.TrackArtist;
          TrackGenre  = value.TrackGenre;
          TrackUrl    = value.TrackUrl;
          selectedBroadcastHistory = value;
        }
      }
    }

    public IEnumerable<ISourceStreamFactory> SourceStreams {
      get { return peerCast.SourceStreamFactories.Where(sstream => (sstream.Type & SourceStreamType.Broadcast)!=0); }
    }
    private ISourceStreamFactory selectedSourceStream;
    public ISourceStreamFactory SelectedSourceStream {
      get { return selectedSourceStream; }
      set {
        if (SetProperty("SelectedSourceStream", ref selectedSourceStream, value) &&
            value!=null &&
            value.DefaultUri!=null) {
          StreamUrl = value.DefaultUri.ToString();
        }
      }
    }

    private string streamUrl = "";
    public string StreamUrl
    {
      get { return streamUrl; }
      set
      {
        SetProperty("StreamUrl", ref streamUrl, value,
          () => start.OnCanExecuteChanged());
      }
    }

    private int? bitrate;
    public string Bitrate
    {
      get { return bitrate.HasValue ? bitrate.ToString() : "自動"; }
      set {
        int result;
        if (!String.IsNullOrEmpty(value) && Int32.TryParse(value, out result)) {
          SetProperty("Bitrate", ref bitrate, result);
        }
        else if (bitrate.HasValue) {
          SetProperty("Bitrate", ref bitrate, null);
        }
      }
    }

    private IContentReaderFactory contentType;
    public IContentReaderFactory ContentType
    {
      get { return contentType; }
      set
      {
        SetProperty("ContentType", ref contentType, value,
          () => start.OnCanExecuteChanged());
      }
    }

    private IYellowPageClient yellowPage;
    public IYellowPageClient YellowPage
    {
      get { return yellowPage; }
      set { SetProperty("YellowPage", ref yellowPage, value); }
    }

    private string channelName = "";
    public string ChannelName
    {
      get { return channelName; }
      set
      {
        SetProperty("ChannelName", ref channelName, value,
          () => start.OnCanExecuteChanged());
      }
    }

    private string genre = "";
    public string Genre
    {
      get { return genre; }
      set { SetProperty("Genre", ref genre, value); }
    }

    private string description = "";
    public string Description
    {
      get { return description; }
      set { SetProperty("Description", ref description, value); }
    }

    private string comment = "";
    public string Comment
    {
      get { return comment; }
      set { SetProperty("Comment", ref comment, value); }
    }

    private string contactUrl = "";
    public string ContactUrl
    {
      get { return contactUrl; }
      set { SetProperty("ContactUrl", ref contactUrl, value); }
    }

    private string trackTitle = "";
    public string TrackTitle
    {
      get { return trackTitle; }
      set { SetProperty("TrackTitle", ref trackTitle, value); }
    }

    private string trackAlbum = "";
    public string TrackAlbum
    {
      get { return trackAlbum; }
      set { SetProperty("TrackAlbum", ref trackAlbum, value); }
    }

    private string trackArtist = "";
    public string TrackArtist
    {
      get { return trackArtist; }
      set { SetProperty("TrackArtist", ref trackArtist, value); }
    }

    private string trackGenre = "";
    public string TrackGenre
    {
      get { return trackGenre; }
      set { SetProperty("TrackGenre", ref trackGenre, value); }
    }

    private string trackUrl = "";
    public string TrackUrl
    {
      get { return trackUrl; }
      set { SetProperty("TrackUrl", ref trackUrl, value); }
    }

    private readonly Command start;
    public ICommand Start { get { return start; } }

    private Uri StreamSource
    {
      get
      {
        Uri source;
        if (!Uri.TryCreate(streamUrl, UriKind.Absolute, out source))
        {
          return null;
        }
        return source;
      }
    }

    private PeerCast peerCast;
    public BroadcastViewModel(PeerCast peerCast)
    {
      this.peerCast = peerCast;
      this.uiSettings = new UISettingsViewModel(PeerCastApplication.Current.Settings);
      start = new Command(OnBroadcast, () => CanBroadcast(StreamSource, ContentType, channelName));
      contentTypes = peerCast.ContentReaderFactories.ToArray();

      yellowPages = Enumerable.Repeat(new KeyValuePair<string,IYellowPageClient>("掲載なし", null),1)
        .Concat(peerCast.YellowPages.Select(yp => new KeyValuePair<string,IYellowPageClient>(yp.Name, yp)));
      if (contentTypes.Length > 0) contentType = contentTypes[0];

      this.SelectedSourceStream = SourceStreams.FirstOrDefault();
    }

    private void OnBroadcast()
    {
      var source = StreamSource;
      var contentReaderFactory = ContentType;
      if (!CanBroadcast(source, contentReaderFactory, channelName)) return;
      IYellowPageClient yellowPage = this.yellowPage;
      var channelInfo = CreateChannelInfo(this);
      var channelTrack = CreateChannelTrack(this);

      var channel_id = PeerCastStation.Core.BroadcastChannel.CreateChannelID(
        peerCast.BroadcastID,
        channelName,
        genre,
        source.ToString());
      var source_stream =
        selectedSourceStream ??
        peerCast.SourceStreamFactories
          .Where(sstream => (sstream.Type & SourceStreamType.Broadcast)!=0)
          .FirstOrDefault(sstream => sstream.Scheme==source.Scheme);
      var channel = peerCast.BroadcastChannel(
        yellowPage,
        channel_id,
        channelInfo,
        source,
        source_stream,
        contentReaderFactory);
      if (channel!=null) {
        channel.ChannelTrack = channelTrack;
      }

      var info = new BroadcastInfoViewModel {
        StreamUrl   = this.StreamUrl,
        StreamType  = this.SelectedSourceStream!=null ? this.SelectedSourceStream.Name : null,
        Bitrate     = this.bitrate.HasValue ? this.bitrate.Value : 0,
        ContentType = this.ContentType.Name,
        YellowPage  = this.YellowPage!=null ? this.YellowPage.Name : null,
        ChannelName = this.ChannelName,
        Genre       = this.Genre,
        Description = this.Description,
        Comment     = this.Comment,
        ContactUrl  = this.ContactUrl,
        TrackTitle  = this.TrackTitle,
        TrackAlbum  = this.TrackAlbum,
        TrackArtist = this.TrackArtist,
        TrackGenre  = this.TrackGenre,
        TrackUrl    = this.TrackUrl,
        Favorite    = false,
      };
      uiSettings.AddBroadcastHistory(info);
      uiSettings.Save();
    }

    public void Save()
    {
      uiSettings.Save();
    }

    private bool CanBroadcast(Uri streamSource, IContentReaderFactory contentReaderFactory, string channelName)
    {
      return streamSource != null
        && contentReaderFactory != null
        && !String.IsNullOrEmpty(channelName);
    }

    private ChannelInfo CreateChannelInfo(BroadcastViewModel viewModel)
    {
      var info = new AtomCollection();
      if (viewModel.bitrate.HasValue) info.SetChanInfoBitrate(viewModel.bitrate.Value);
      info.SetChanInfoName(viewModel.channelName);
      info.SetChanInfoGenre(viewModel.genre);
      info.SetChanInfoDesc(viewModel.description);
      info.SetChanInfoComment(viewModel.comment);
      info.SetChanInfoURL(viewModel.contactUrl);
      return new ChannelInfo(info);
    }

    private ChannelTrack CreateChannelTrack(BroadcastViewModel viewModel)
    {
      var collection = new AtomCollection();
      collection.SetChanTrackTitle(viewModel.TrackTitle);
      collection.SetChanTrackGenre(viewModel.TrackGenre);
      collection.SetChanTrackAlbum(viewModel.TrackAlbum);
      collection.SetChanTrackCreator(viewModel.TrackArtist);
      collection.SetChanTrackURL(viewModel.TrackUrl);
      return new ChannelTrack(collection);
    }
  }
}

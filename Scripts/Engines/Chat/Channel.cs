using System;
using System.Collections;
using Server;

namespace Server.Engines.Chat
{
	public class Channel
	{
		private string m_Name;
		private string m_Password;
		private ArrayList m_Users, m_Banned, m_Moderators, m_Voices;
		private bool m_VoiceRestricted;
		private bool m_AlwaysAvailable;

		public Channel( string name )
		{
			m_Name = name;

			m_Users = new ArrayList();
			m_Banned = new ArrayList();
			m_Moderators = new ArrayList();
			m_Voices = new ArrayList();
		}

		public Channel( string name, string password ) : this( name )
		{
			m_Password = password;
		}

		public string Name
		{
			get { return m_Name; }
			set
			{
				SendCommand( ChatCommand.RemoveChannel, m_Name );
				m_Name = value;
				SendCommand( ChatCommand.AddChannel, m_Name );
				SendCommand( ChatCommand.JoinedChannel, m_Name );
			}
		}

		public string Password
		{
			get { return m_Password; }
			set
			{
				string newValue = null;

				if ( value != null )
				{
					newValue = value.Trim();

					if ( newValue == null || newValue == String.Empty )
					{
						newValue = null;
					}
				}

				m_Password = newValue;
			}
		}

		public bool Contains( ChatUser user )
		{
			return m_Users.Contains( user );
		}

		public bool IsBanned( ChatUser user )
		{
			return m_Banned.Contains( user );
		}

		public bool CanTalk( ChatUser user )
		{
			return (!m_VoiceRestricted || m_Voices.Contains( user ) || m_Moderators.Contains( user ));
		}

		public bool IsModerator( ChatUser user )
		{
			return m_Moderators.Contains( user );
		}

		public bool IsVoiced( ChatUser user )
		{
			return m_Voices.Contains( user );
		}

		public bool ValidatePassword( string password )
		{
			return (m_Password == null || Insensitive.Equals( m_Password, password ));
		}

		public bool ValidateModerator( ChatUser user )
		{
			if ( user != null && !IsModerator( user ) )
			{
				user.SendMessage( 29 ); // You must have operator status to do this.
				return false;
			}

			return true;
		}

		public bool ValidateAccess( ChatUser from, ChatUser target )
		{
			if ( from != null && target != null && from.Mobile.AccessLevel < target.Mobile.AccessLevel )
			{
				from.Mobile.SendMessage( "Your access level is too low to do this." );
				return false;
			}

			return true;
		}

		public bool AddUser( ChatUser user )
		{
			return AddUser( user, null );
		}

		public bool AddUser( ChatUser user, string password )
		{
			if ( Contains( user ) )
			{
				user.SendMessage( 46, m_Name ); // You are already in the conference '%1'.
				return true;
			}
			else if ( IsBanned( user ) )
			{
				user.SendMessage( 64 ); // You have been banned from this conference.
				return false;
			}
			else if ( !ValidatePassword( password ) )
			{
				user.SendMessage( 34 ); // That is not the correct password.
				return false;
			}
			else
			{
				if ( user.CurrentChannel != null )
				{
					// Remove them from their current channel first
					user.CurrentChannel.RemoveUser( user );
				} 

				ChatSystem.SendCommandTo( user.Mobile, ChatCommand.JoinedChannel, m_Name );

				SendCommand( ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username );

				m_Users.Add( user );
				user.CurrentChannel = this;

				if ( user.Mobile.AccessLevel >= AccessLevel.GameMaster || (!m_AlwaysAvailable && m_Users.Count == 1) )
				{
					AddModerator( user );
				}

				SendUsersTo( user );

				return true;
			}
		}

		public void RemoveUser( ChatUser user )
		{
			if ( Contains( user ) )
			{
				m_Users.Remove( user );
				user.CurrentChannel = null;

				if ( m_Moderators.Contains( user ) )
				{
					m_Moderators.Remove( user );
				}

				if ( m_Voices.Contains( user ) )
				{
					m_Voices.Remove( user );
				}

				SendCommand( ChatCommand.RemoveUserFromChannel, user, user.Username );
				ChatSystem.SendCommandTo( user.Mobile, ChatCommand.LeaveChannel );

				if ( m_Users.Count == 0 && !m_AlwaysAvailable )
				{
					RemoveChannel( this );
				}
			}
		}

		public void AdBan( ChatUser user )
		{
			AddBan( user, null );
		}

		public void AddBan( ChatUser user, ChatUser moderator )
		{
			if ( !ValidateModerator( moderator ) || !ValidateAccess( moderator, user ) )
			{
				return;
			}

			if ( !m_Banned.Contains( user ) )
			{
				m_Banned.Add( user );
			}

			Kick( user, moderator, true );
		}

		public void RemoveBan( ChatUser user )
		{
			if ( m_Banned.Contains( user ) )
			{
				m_Banned.Remove( user );
			}
		}

		public void Kick( ChatUser user )
		{
			Kick( user, null );
		}

		public void Kick( ChatUser user, ChatUser moderator )
		{
			Kick( user, moderator, false );
		}

		public void Kick( ChatUser user, ChatUser moderator, bool wasBanned )
		{
			if ( !ValidateModerator( moderator ) || !ValidateAccess( moderator, user ) )
			{
				return;
			}

			if ( Contains( user ) )
			{
				if ( moderator != null )
				{
					if ( wasBanned )
					{
						// %1, a conference moderator, has banned you from the conference.
						user.SendMessage( 63, moderator.Username );
					} 
					else
					{
						// %1, a conference moderator, has kicked you out of the conference.
						user.SendMessage( 45, moderator.Username );
					} 
				}

				RemoveUser( user );
				ChatSystem.SendCommandTo( user.Mobile, ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username );

				SendMessage( 44, user.Username ); // %1 has been kicked out of the conference.
			}

			if ( wasBanned && moderator != null )
			{
				// You are banning %1 from this conference.
				moderator.SendMessage( 62, user.Username );
			} 
		}

		public bool VoiceRestricted
		{
			get { return m_VoiceRestricted; }
			set
			{
				m_VoiceRestricted = value;

				if ( value )
				{
					// From now on, only moderators will have speaking privileges in this conference by default.
					SendMessage( 56 );
				} 
				else
				{
					// From now on, everyone in the conference will have speaking privileges by default.
					SendMessage( 55 );
				} 
			}
		}

		public bool AlwaysAvailable { get { return m_AlwaysAvailable; } set { m_AlwaysAvailable = value; } }

		public void AddVoiced( ChatUser user )
		{
			AddVoiced( user, null );
		}

		public void AddVoiced( ChatUser user, ChatUser moderator )
		{
			if ( !ValidateModerator( moderator ) )
			{
				return;
			}

			if ( !IsBanned( user ) && !IsModerator( user ) && !IsVoiced( user ) )
			{
				m_Voices.Add( user );

				if ( moderator != null )
				{
					// %1, a conference moderator, has granted you speaking priviledges in this conference.
					user.SendMessage( 54, moderator.Username );
				} 

				SendMessage( 52, user, user.Username ); // %1 now has speaking privileges in this conference.
				SendCommand( ChatCommand.AddUserToChannel, user, user.GetColorCharacter() + user.Username );
			}
		}

		public void RemoveVoiced( ChatUser user, ChatUser moderator )
		{
			if ( !ValidateModerator( moderator ) || !ValidateAccess( moderator, user ) )
			{
				return;
			}

			if ( !IsModerator( user ) && IsVoiced( user ) )
			{
				m_Voices.Remove( user );

				if ( moderator != null )
				{
					// %1, a conference moderator, has removed your speaking priviledges for this conference.
					user.SendMessage( 53, moderator.Username );
				} 

				SendMessage( 51, user, user.Username ); // %1 no longer has speaking privileges in this conference.
				SendCommand( ChatCommand.AddUserToChannel, user, user.GetColorCharacter() + user.Username );
			}
		}

		public void AddModerator( ChatUser user )
		{
			AddModerator( user, null );
		}

		public void AddModerator( ChatUser user, ChatUser moderator )
		{
			if ( !ValidateModerator( moderator ) )
			{
				return;
			}

			if ( IsBanned( user ) || IsModerator( user ) )
			{
				return;
			}

			if ( IsVoiced( user ) )
			{
				m_Voices.Remove( user );
			}

			m_Moderators.Add( user );

			if ( moderator != null )
			{
				// %1 has made you a conference moderator.
				user.SendMessage( 50, moderator.Username );
			} 

			SendMessage( 48, user, user.Username ); // %1 is now a conference moderator.
			SendCommand( ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username );
		}

		public void RemoveModerator( ChatUser user )
		{
			RemoveModerator( user, null );
		}

		public void RemoveModerator( ChatUser user, ChatUser moderator )
		{
			if ( !ValidateModerator( moderator ) || !ValidateAccess( moderator, user ) )
			{
				return;
			}

			if ( IsModerator( user ) )
			{
				m_Moderators.Remove( user );

				if ( moderator != null )
				{
					// %1 has removed you from the list of conference moderators.
					user.SendMessage( 49, moderator.Username );
				} 

				SendMessage( 47, user, user.Username ); // %1 is no longer a conference moderator.
				SendCommand( ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username );
			}
		}

		public void SendMessage( int number )
		{
			SendMessage( number, null, null, null );
		}

		public void SendMessage( int number, string param1 )
		{
			SendMessage( number, null, param1, null );
		}

		public void SendMessage( int number, string param1, string param2 )
		{
			SendMessage( number, null, param1, param2 );
		}

		public void SendMessage( int number, ChatUser initiator )
		{
			SendMessage( number, initiator, null, null );
		}

		public void SendMessage( int number, ChatUser initiator, string param1 )
		{
			SendMessage( number, initiator, param1, null );
		}

		public void SendMessage( int number, ChatUser initiator, string param1, string param2 )
		{
			for ( int i = 0; i < m_Users.Count; ++i )
			{
				ChatUser user = (ChatUser) m_Users[ i ];

				if ( user == initiator )
				{
					continue;
				}

				if ( user.CheckOnline() )
				{
					user.SendMessage( number, param1, param2 );
				}
				else if ( !Contains( user ) )
				{
					--i;
				}
			}
		}

		public void SendIgnorableMessage( int number, ChatUser from, string param1, string param2 )
		{
			for ( int i = 0; i < m_Users.Count; ++i )
			{
				ChatUser user = (ChatUser) m_Users[ i ];

				if ( user.IsIgnored( from ) )
				{
					continue;
				}

				if ( user.CheckOnline() )
				{
					user.SendMessage( number, from.Mobile, param1, param2 );
				}
				else if ( !Contains( user ) )
				{
					--i;
				}
			}
		}

		public void SendCommand( ChatCommand command )
		{
			SendCommand( command, null, null, null );
		}

		public void SendCommand( ChatCommand command, string param1 )
		{
			SendCommand( command, null, param1, null );
		}

		public void SendCommand( ChatCommand command, string param1, string param2 )
		{
			SendCommand( command, null, param1, param2 );
		}

		public void SendCommand( ChatCommand command, ChatUser initiator )
		{
			SendCommand( command, initiator, null, null );
		}

		public void SendCommand( ChatCommand command, ChatUser initiator, string param1 )
		{
			SendCommand( command, initiator, param1, null );
		}

		public void SendCommand( ChatCommand command, ChatUser initiator, string param1, string param2 )
		{
			for ( int i = 0; i < m_Users.Count; ++i )
			{
				ChatUser user = (ChatUser) m_Users[ i ];

				if ( user == initiator )
				{
					continue;
				}

				if ( user.CheckOnline() )
				{
					ChatSystem.SendCommandTo( user.Mobile, command, param1, param2 );
				}
				else if ( !Contains( user ) )
				{
					--i;
				}
			}
		}

		public void SendUsersTo( ChatUser to )
		{
			for ( int i = 0; i < m_Users.Count; ++i )
			{
				ChatUser user = (ChatUser) m_Users[ i ];

				ChatSystem.SendCommandTo( to.Mobile, ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username );
			}
		}

		private static ArrayList m_Channels = new ArrayList();

		public static ArrayList Channels { get { return m_Channels; } }

		public static void SendChannelsTo( ChatUser user )
		{
			for ( int i = 0; i < m_Channels.Count; ++i )
			{
				Channel channel = (Channel) m_Channels[ i ];

				if ( !channel.IsBanned( user ) )
				{
					ChatSystem.SendCommandTo( user.Mobile, ChatCommand.AddChannel, channel.Name, "0" );
				}
			}
		}

		public static Channel AddChannel( string name )
		{
			return AddChannel( name, null );
		}

		public static Channel AddChannel( string name, string password )
		{
			Channel channel = FindChannelByName( name );

			if ( channel == null )
			{
				channel = new Channel( name, password );
				m_Channels.Add( channel );
			}

			ChatUser.GlobalSendCommand( ChatCommand.AddChannel, name, "0" );

			return channel;
		}

		public static void RemoveChannel( string name )
		{
			RemoveChannel( FindChannelByName( name ) );
		}

		public static void RemoveChannel( Channel channel )
		{
			if ( channel == null )
			{
				return;
			}

			if ( m_Channels.Contains( channel ) && channel.m_Users.Count == 0 )
			{
				ChatUser.GlobalSendCommand( ChatCommand.RemoveChannel, channel.Name );

				channel.m_Moderators.Clear();
				channel.m_Voices.Clear();

				m_Channels.Remove( channel );
			}
		}

		public static Channel FindChannelByName( string name )
		{
			for ( int i = 0; i < m_Channels.Count; ++i )
			{
				Channel channel = (Channel) m_Channels[ i ];

				if ( channel.m_Name == name )
				{
					return channel;
				}
			}

			return null;
		}

		public static void Initialize()
		{
			AddStaticChannel( "Newbie Help" );
		}

		public static void AddStaticChannel( string name )
		{
			AddChannel( name ).AlwaysAvailable = true;
		}
	}
}
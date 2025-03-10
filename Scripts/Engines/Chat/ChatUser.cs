using System;
using System.Collections;
using Server;
using Server.Accounting;

namespace Server.Engines.Chat
{
	public class ChatUser
	{
		private Mobile m_Mobile;
		private Channel m_Channel;
		private bool m_Anonymous;
		private bool m_IgnorePrivateMessage;
		private ArrayList m_Ignored, m_Ignoring;

		public ChatUser( Mobile m )
		{
			m_Mobile = m;
			m_Ignored = new ArrayList();
			m_Ignoring = new ArrayList();
		}

		public Mobile Mobile { get { return m_Mobile; } }

		public ArrayList Ignored { get { return m_Ignored; } }

		public ArrayList Ignoring { get { return m_Ignoring; } }

		public string Username
		{
			get
			{
				Account acct = m_Mobile.Account as Account;

				if ( acct != null )
				{
					return acct.GetTag( "ChatName" );
				}

				return null;
			}
			set
			{
				Account acct = m_Mobile.Account as Account;

				if ( acct != null )
				{
					acct.SetTag( "ChatName", value );
				}
			}
		}

		public Channel CurrentChannel { get { return m_Channel; } set { m_Channel = value; } }

		public bool IsOnline { get { return (m_Mobile.NetState != null); } }

		public bool Anonymous { get { return m_Anonymous; } set { m_Anonymous = value; } }

		public bool IgnorePrivateMessage { get { return m_IgnorePrivateMessage; } set { m_IgnorePrivateMessage = value; } }

		public const char NormalColorCharacter = '0';
		public const char ModeratorColorCharacter = '1';
		public const char VoicedColorCharacter = '2';

		public char GetColorCharacter()
		{
			if ( m_Channel != null && m_Channel.IsModerator( this ) )
			{
				return ModeratorColorCharacter;
			}

			if ( m_Channel != null && m_Channel.IsVoiced( this ) )
			{
				return VoicedColorCharacter;
			}

			return NormalColorCharacter;
		}

		public bool CheckOnline()
		{
			if ( IsOnline )
			{
				return true;
			}

			RemoveChatUser( this );
			return false;
		}

		public void SendMessage( int number )
		{
			SendMessage( number, null, null );
		}

		public void SendMessage( int number, string param1 )
		{
			SendMessage( number, param1, null );
		}

		public void SendMessage( int number, string param1, string param2 )
		{
			if ( m_Mobile.NetState != null )
			{
				m_Mobile.Send( new ChatMessagePacket( m_Mobile, number, param1, param2 ) );
			}
		}

		public void SendMessage( int number, Mobile from, string param1, string param2 )
		{
			if ( m_Mobile.NetState != null )
			{
				m_Mobile.Send( new ChatMessagePacket( from, number, param1, param2 ) );
			}
		}

		public bool IsIgnored( ChatUser check )
		{
			return m_Ignored.Contains( check );
		}

		public bool IsModerator { get { return (m_Channel != null && m_Channel.IsModerator( this )); } }

		public void AddIgnored( ChatUser user )
		{
			if ( IsIgnored( user ) )
			{
				SendMessage( 22, user.Username ); // You are already ignoring %1.
			}
			else
			{
				m_Ignored.Add( user );
				user.m_Ignoring.Add( this );

				SendMessage( 23, user.Username ); // You are now ignoring %1.
			}
		}

		public void RemoveIgnored( ChatUser user )
		{
			if ( IsIgnored( user ) )
			{
				m_Ignored.Remove( user );
				user.m_Ignoring.Remove( this );

				SendMessage( 24, user.Username ); // You are no longer ignoring %1.

				if ( m_Ignored.Count == 0 )
				{
					// You are no longer ignoring anyone.
					SendMessage( 26 );
				}
			}
			else
			{
				SendMessage( 25, user.Username ); // You are not ignoring %1.
			}
		}

		private static ArrayList m_Users = new ArrayList();
		private static Hashtable m_Table = new Hashtable();

		public static ChatUser AddChatUser( Mobile from )
		{
			ChatUser user = GetChatUser( from );

			if ( user == null )
			{
				user = new ChatUser( from );

				m_Users.Add( user );
				m_Table[ from ] = user;

				Channel.SendChannelsTo( user );

				ArrayList list = Channel.Channels;

				for ( int i = 0; i < list.Count; ++i )
				{
					Channel c = (Channel) list[ i ];

					if ( c.AddUser( user ) )
					{
						break;
					}
				}

				//ChatSystem.SendCommandTo( user.m_Mobile, ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username );
			}

			return user;
		}

		public static void RemoveChatUser( ChatUser user )
		{
			if ( user == null )
			{
				return;
			}

			for ( int i = 0; i < user.m_Ignoring.Count; ++i )
			{
				((ChatUser) user.m_Ignoring[ i ]).RemoveIgnored( user );
			}

			if ( m_Users.Contains( user ) )
			{
				ChatSystem.SendCommandTo( user.Mobile, ChatCommand.CloseChatWindow );

				if ( user.m_Channel != null )
				{
					user.m_Channel.RemoveUser( user );
				}

				m_Users.Remove( user );
				m_Table.Remove( user.m_Mobile );
			}
		}

		public static void RemoveChatUser( Mobile from )
		{
			ChatUser user = GetChatUser( from );

			RemoveChatUser( user );
		}

		public static ChatUser GetChatUser( Mobile from )
		{
			return (ChatUser) m_Table[ from ];
		}

		public static ChatUser GetChatUser( string username )
		{
			for ( int i = 0; i < m_Users.Count; ++i )
			{
				ChatUser user = (ChatUser) m_Users[ i ];

				if ( user.Username == username )
				{
					return user;
				}
			}

			return null;
		}

		public static void GlobalSendCommand( ChatCommand command )
		{
			GlobalSendCommand( command, null, null, null );
		}

		public static void GlobalSendCommand( ChatCommand command, string param1 )
		{
			GlobalSendCommand( command, null, param1, null );
		}

		public static void GlobalSendCommand( ChatCommand command, string param1, string param2 )
		{
			GlobalSendCommand( command, null, param1, param2 );
		}

		public static void GlobalSendCommand( ChatCommand command, ChatUser initiator )
		{
			GlobalSendCommand( command, initiator, null, null );
		}

		public static void GlobalSendCommand( ChatCommand command, ChatUser initiator, string param1 )
		{
			GlobalSendCommand( command, initiator, param1, null );
		}

		public static void GlobalSendCommand( ChatCommand command, ChatUser initiator, string param1, string param2 )
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
					ChatSystem.SendCommandTo( user.m_Mobile, command, param1, param2 );
				}
				else if ( !m_Users.Contains( i ) )
				{
					--i;
				}
			}
		}
	}
}
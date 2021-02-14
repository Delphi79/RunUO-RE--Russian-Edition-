using System;
using Server;

namespace Server.Prompts
{
	public delegate void DelegatePromptCallback( Mobile from, bool cancel, string text, object state );

	public class DelegatePrompt : Prompt
	{
		private DelegatePromptCallback m_Callback;
		private object m_State;

		public DelegatePrompt( DelegatePromptCallback callback, object state )
		{
			m_Callback = callback;
			m_State = state;
		}

		public override void OnCancel( Mobile from )
		{
			m_Callback( from, true, "", m_State );
		}

		public override void OnResponse( Mobile from, string text )
		{
			m_Callback( from, false, text, m_State );
		}
	}
}
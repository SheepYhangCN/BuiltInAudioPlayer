using Godot;
using System;
using System.Collections;
using System.Linq;

public partial class Main : Control
{
	string[] locales;

	bool addition_label = true;
	bool pauseable = true;
	bool allow_selecting = true;
	string force_locale = "";
	string locale="";

	AudioStream audio;
	bool paused = false;
	public override void _Ready()
	{
		if (FileAccess.FileExists("res://options.ini"))
		{
			var file=new ConfigFile();
			Error load = file.Load("res://options.ini");
			if (load == Error.Ok)
			{
				addition_label=(bool)file.GetValue("Options","AdditionLabel",true);
				pauseable=(bool)file.GetValue("Options","Pauseble",true);
				allow_selecting=(bool)file.GetValue("Options","AllowSelecting",true);
				locale=(string)file.GetValue("UserOptions","Locale","");
				force_locale=(string)file.GetValue("Options","ForceLocale","");
			}
			else
			{
				GD.PushError($"Options file load failed, error code: {(int)load}("+load.ToString()+")");
			}
		}
		else
		{
			var file=new ConfigFile();
			file.SetValue("Options","AdditionLabel",addition_label);
			file.SetValue("Options","Pauseble",pauseable);
			file.SetValue("Options","AllowSelecting",allow_selecting);
			file.SetValue("Options","ForceLocale",force_locale);
			file.SetValue("UserOptions","Locale",locale);
			Error save = file.Save("res://options.ini");
			if (save != Error.Ok)
			{
				GD.PushError($"Options file create failed, error code: {(int)save}("+save.ToString()+")");
			}
		}
		if (FileAccess.FileExists("user://user_options.ini"))
		{
			var file=new ConfigFile();
			Error load = file.Load("user://user_options.ini");
			if (load == Error.Ok)
			{
				locale=(string)file.GetValue("UserOptions","Locale","");
			}
			else
			{
				GD.PushError($"User Options file load failed, error code: {(int)load}("+load.ToString()+")");
			}
		}
		else
		{
			var file=new ConfigFile();
			file.SetValue("UserOptions","Locale",locale);
			Error save = file.Save("user://user_options.ini");
			if (save != Error.Ok)
			{
				GD.PushError($"User Options file create failed, error code: {(int)save}("+save.ToString()+")");
			}
		}

		Load();

		if (OS.GetLocale() == "zh_TW" || OS.GetLocale() == "zh_HK" || OS.GetLocale() == "zh_MO")
		{
		    TranslationServer.SetLocale("zh_TW");
		}
		else if (OS.GetLocaleLanguage() == "zh" || OS.GetLocale() == "zh_CN" || OS.GetLocale() == "zh_SG")
		{
		    TranslationServer.SetLocale("zh_CN");
		}
		else
		{
		    TranslationServer.SetLocale(OS.GetLocale());
		}
		var node=GetNode<OptionButton>("Languages");
		var csv="res://Locale/locale.csv";
		var ff=FileAccess.Open(csv,FileAccess.ModeFlags.Read);
		var err=FileAccess.GetOpenError();
		if (err == Error.Ok)
		{
			var list=new ArrayList(ff.GetCsvLine(","));
			list.RemoveAt(0);
			locales=(string[])list.ToArray(typeof(string));
			ff.Close();
		}
		else
		{
			GD.PushError($"Localization file load failed, error code: {(int)err}("+err.ToString()+")");
		}
		node.ItemCount=locales.Length;
		foreach (var current in locales)
		{
			node.Set("popup/item_"+Array.IndexOf(locales,current).ToString()+"/text",TranslationServer.GetTranslationObject(current).GetMessage("locLanguageName"));
		}
		var filee=new ConfigFile();
		Error loadd = filee.Load("user://user_options.ini");
		if (loadd == Error.Ok)
		{
			if ((string)filee.GetValue("UserOptions","Locale","") == "")
			{
				filee.SetValue("UserOptions","Locale",TranslationServer.GetLocale());
				Error savee = filee.Save("user://user_options.ini");
				if (savee != Error.Ok)
				{
					GD.PushError($"User Options file save failed, error code: {(int)savee}("+savee.ToString()+")");
				}
			}
			else
			{
				TranslationServer.SetLocale((string)filee.GetValue("UserOptions","Locale","en"));
			}
		}
		else
		{
			GD.PushError($"User Options file load failed, error code: {(int)loadd}("+loadd.ToString()+")");
		}
		if (force_locale != "")
		{
		    TranslationServer.SetLocale(force_locale);
		}
		node.Selected=Array.IndexOf(locales.ToArray(),locales.Contains(TranslationServer.GetLocale()) ? TranslationServer.GetLocale() : TranslationServer.GetLocale().Left(2));
	}

	public override void _Process(double delta)
	{
		GetNode<OptionButton>("Languages").Visible=(force_locale == "");
		var pause=GetNode<Button>("CenterContainer/VBoxContainer/HBoxContainer/Pause");
		pause.Text=paused ? "locResume" : "locPause";
		pause.Visible=pauseable;
		pause.Disabled=!(GetNode<AudioStreamPlayer>("AudioStreamPlayer").Playing) && !(GetNode<AudioStreamPlayer>("AudioStreamPlayer").StreamPaused);
		var status=TranslationServer.Translate("locNotLoaded");
		if (audio is AudioStreamOggVorbis)
		{
			status=TranslationServer.Translate("locLoaded")+" (OGG)";
		}
		if (audio is AudioStreamWav)
		{
			status=TranslationServer.Translate("locLoaded")+" (WAV)";
		}
		if (audio is AudioStreamMP3)
		{
			status=TranslationServer.Translate("locLoaded")+" (MP3)";
		}
		GetNode<Label>("CenterContainer/VBoxContainer/Label").Text=(addition_label ? TranslationServer.Translate("locLabel1") : "")+TranslationServer.Translate("locLabel")+status;
	}
	
	public void _on_languages_item_selected(int selected)
	{
		TranslationServer.SetLocale(locales[selected]);
		var file=new ConfigFile();
		Error load = file.Load("user://user_options.ini");
		if (load == Error.Ok)
		{
			file.SetValue("UserOptions","Locale",TranslationServer.GetLocale());
			Error save = file.Save("user://user_options.ini");
			if (save != Error.Ok)
			{
				GD.PushError($"User Options file save failed, error code: {(int)save}("+save.ToString()+")");
			}
		}
		else
		{
			GD.PushError($"User Options file load failed, error code: {(int)load}("+load.ToString()+")");
		}
	}

	public void _on_play_pressed()
	{
		paused=false;
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
	}

	public void _on_pause_pressed()
	{
		paused=!paused;
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").StreamPaused=paused;
	}

	public void _on_stop_pressed()
	{
		paused=false;
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").StreamPaused=false;
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stop();
	}

	public void _on_reload_pressed()
	{
		paused=false;
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").StreamPaused=false;
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stop();
		Load();
	}

	void Load()
	{
		if (FileAccess.FileExists("res://audio.ogg"))
		{
			audio=AudioStreamOggVorbis.LoadFromFile("res://audio.ogg");
			GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stream=audio;
		}
		else if (FileAccess.FileExists("res://audio.wav"))
		{
			var f=FileAccess.Open("res://audio.wav",FileAccess.ModeFlags.Read);
			audio=new AudioStreamWav
			{
				Data = f.GetBuffer((long)f.GetLength()),
				Format = AudioStreamWav.FormatEnum.Format16Bits
			};
			f.Close();
		}
		else if (FileAccess.FileExists("res://audio.mp3"))
		{
			var file=FileAccess.Open("res://audio.mp3",FileAccess.ModeFlags.Read);
			audio = new AudioStreamMP3
			{
				Data = file.GetBuffer((long)file.GetLength())
			};
			GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stream=audio;
			file.Close();
		}
		else if (allow_selecting)
		{
			var window=new FileDialog();
			window.Access=FileDialog.AccessEnum.Filesystem;
			window.UseNativeDialog=true;
			window.FileMode=FileDialog.FileModeEnum.OpenFile;
			window.Filters=["*.ogg ; OGG"+TranslationServer.Translate("locAudioFile"),"*.wav ; WAV"+TranslationServer.Translate("locAudioFile"),"*.mp3 ; MP3"+TranslationServer.Translate("locAudioFile")];
			window.CurrentDir=OS.GetExecutablePath().Substring(0,OS.GetExecutablePath().LastIndexOf("/"));
			window.FileSelected+=Selected;
			window.Visible=true;
		}
	}

	void Selected(string file)
	{
		if (file.ToLower().EndsWith(".ogg"))
		{
			audio=AudioStreamOggVorbis.LoadFromFile(file);
		}
		else if (file.ToLower().EndsWith(".wav"))
		{
			var f=FileAccess.Open(file,FileAccess.ModeFlags.Read);
			audio = new AudioStreamWav
			{
				Data = f.GetBuffer((long)f.GetLength()),
				Format = AudioStreamWav.FormatEnum.Format16Bits
			};
			f.Close();
		}
		else if (file.ToLower().EndsWith(".mp3"))
		{
			var f=FileAccess.Open(file,FileAccess.ModeFlags.Read);
			audio = new AudioStreamMP3
			{
				Data = f.GetBuffer((long)f.GetLength())
			};
			f.Close();
		}
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stream=audio;
	}
}
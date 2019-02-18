using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

namespace Prashant
{
    public class GameConfigData
    {
        public string projectName;
        public string companyName;
        public string gameVersion;
        public string bundleVersionCode;
        public string gameVersionWhenSignedWithKeystore;
        public string bundleVersionCodeWhenSignedWithkeystore;
        public string GameBundleIdentifier;
        public string keystoreAliasName;
        public string keystorePassword;
        public string keystoreAliasPassword;
        public string keystorePath;
        public string unityVersion;
        public string lastBuildDate;
        public string lastSignedBuildDate;
    }

    class ProjectWizard : EditorWindow
    {
        static GUIStyle styleHelpboxInner;
        static GUIStyle titleLabel, normalLabel, subtitleLabel;
        static string m_autoIncreaseVersion = "shouldAutoIncreaseVersion";
        static string m_autoRunKey = "shouldAutoRun";
        static string m_autoIncrementKey = "shouldAutoIncreaseKey";
        static string m_autoIncrementValue;
        static bool keystoreFileExists;
        static Texture2D configScriptIcon;
        static string toolVersion = "0.1";

        static void InitStyles()
        {
            titleLabel = new GUIStyle();
            titleLabel.fontSize = 11;
            titleLabel.normal.textColor = Color.white;
            titleLabel.alignment = TextAnchor.UpperCenter;
            titleLabel.fixedHeight = 12;

            normalLabel = new GUIStyle();
            normalLabel.fontSize = 11;
            normalLabel.normal.textColor = Color.white;
            normalLabel.fixedHeight = 12;
            normalLabel.alignment = TextAnchor.MiddleCenter;

            subtitleLabel = new GUIStyle();
            subtitleLabel.fontSize = 14;
            subtitleLabel.normal.textColor = Color.white;
            subtitleLabel.fixedHeight = 15;
            subtitleLabel.alignment = TextAnchor.MiddleLeft;

            styleHelpboxInner = new GUIStyle("HelpBox");
            styleHelpboxInner.padding = new RectOffset(4, 4, 4, 4);
        }



        [MenuItem("[Master_Tools]/Project Wizard %W")]
        static void Init()
        {
            shouldAutoIncrement = EditorPrefs.GetBool(m_autoIncreaseVersion);
            shouldAutoRun = EditorPrefs.GetBool(m_autoRunKey);
            m_autoIncrementValue = EditorPrefs.GetString(m_autoIncrementKey);
            EditorWindow window = (ProjectWizard)EditorWindow.GetWindow(typeof(ProjectWizard), true, "Project Configurations");
            configScriptIcon = EditorGUIUtility.FindTexture("GameManager Icon");
            window.minSize = new Vector2(400, 420);
            window.maxSize = new Vector2(500, 420);
            window.titleContent = new GUIContent("Project Wizard");
            RefreshKeystoreValues();
            InitStyles();
        }

        static void RefreshKeystoreValues()
        {
            LoadOldData();
            if (_config.keystorePath.Length > 0)
            {
                keystoreFileExists = (!File.Exists(_config.keystorePath));
                string[] fileName = _config.keystorePath.Split('/');
                string environmentPath = Environment.CurrentDirectory.Replace("\\", "/");
                if (File.Exists(environmentPath + "/" + fileName[fileName.Length - 1]))
                {
                    keystoreFileExists = true;
                    _config.keystorePath = environmentPath + "/" + fileName[fileName.Length - 1];
                }
                if (!keystoreFileExists && PlayerSettings.Android.keystoreName.Length > 0)
                {
                    _config.keystorePath = PlayerSettings.Android.keystoreName;
                }
            }
            keystorePath = _config.keystorePath.Length > 0 ? _config.keystorePath : PlayerSettings.Android.keystoreName;
            projectName = _config.projectName;
            companyName = _config.companyName;
            currentBundleIdentifier = _config.GameBundleIdentifier;
            keystorePassword = _config.keystorePassword.Length > 0 ? _config.keystorePassword : PlayerSettings.Android.keystorePass;
            keystoreAliasPass = _config.keystoreAliasPassword.Length > 0 ? _config.keystoreAliasPassword : PlayerSettings.Android.keyaliasPass;
            keystoreAliasName = _config.keystoreAliasName.Length > 0 ? _config.keystoreAliasName : PlayerSettings.Android.keyaliasName;
        }




        static void GatherDataFromEditor()
        {
            _config = new GameConfigData();
            _config.companyName = PlayerSettings.companyName;
            _config.projectName = PlayerSettings.productName;
            _config.GameBundleIdentifier = PlayerSettings.applicationIdentifier;
            _config.keystoreAliasName = PlayerSettings.Android.keyaliasName;
            _config.keystoreAliasPassword = PlayerSettings.Android.keyaliasPass;
            _config.keystorePassword = PlayerSettings.Android.keystorePass;
            _config.keystorePath = PlayerSettings.Android.keystoreName;
        }

        static void LoadOldData()
        {
            var filePath = Environment.CurrentDirectory + "/Assets/" + "GameConfig.json";
            if (File.Exists(filePath))
            {
                WWW www = new WWW(filePath);
                while (!www.isDone) ;
                _config = JsonUtility.FromJson<GameConfigData>(www.text);
            }
            else
            {
                GatherDataFromEditor();
                var _data = JsonUtility.ToJson(_config);
                SaveDataPhysically(_data);
            }
        }

        static GameConfigData _config;
        static string currentBundleIdentifier;
        static string keystorePassword, keystoreAliasPass, keystoreAliasName, keystorePath;
        static string projectName, companyName;
        static bool isIdentifierSet;
        static bool shouldAutoIncrement;
        static bool shouldAutoRun;
        // static string keystoreFilePath;
        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// This function can be called multiple times per frame (one call per event).
        /// </summary>
        void OnGUI()
        {
            EditorGUILayout.BeginVertical(styleHelpboxInner);
            EditorGUILayout.BeginVertical(styleHelpboxInner);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(toolVersion, GUILayout.MaxWidth(50));
            EditorGUILayout.LabelField("Project Details", titleLabel, new GUILayoutOption[0]);
            if (GUILayout.Button(configScriptIcon, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
            {
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(Environment.CurrentDirectory + "/Assets/" + "GameConfig.json", 0);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Project Name", new GUILayoutOption[0]);
            projectName = EditorGUILayout.TextField(projectName, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Company Name", new GUILayoutOption[0]);
            companyName = EditorGUILayout.TextField(companyName, new GUILayoutOption[0]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bundle Identifier", new GUILayoutOption[0]);
            currentBundleIdentifier = EditorGUILayout.TextField(currentBundleIdentifier, new GUILayoutOption[0]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (!IsIdentifierSet())
            {
                EditorGUILayout.HelpBox("Please change the bundle Identifier!", MessageType.Warning);
            }

            EditorGUILayout.BeginVertical(styleHelpboxInner);

            EditorGUILayout.LabelField("Keystore Details", titleLabel);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.SelectableLabel(keystorePath, new GUILayoutOption[0]);
            if (GUILayout.Button("Select Keystore File", new GUILayoutOption[0]))
            {
                string path = EditorUtility.OpenFilePanel("Select Keystore File", Environment.CurrentDirectory, "keystore");
                if (path.Length > 0)
                {
                    keystorePath = path;
                    Debug.Log("KeystorePath " + keystorePath + " path " + path);
                }
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Alias Name", new GUILayoutOption[0]);
            keystoreAliasName = EditorGUILayout.TextField(keystoreAliasName, new GUILayoutOption[0]).ToLower();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Alias Password", new GUILayoutOption[0]);
            keystoreAliasPass = EditorGUILayout.TextField(keystoreAliasPass, new GUILayoutOption[0]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Password", new GUILayoutOption[0]);
            keystorePassword = EditorGUILayout.TextField(keystorePassword, new GUILayoutOption[0]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(styleHelpboxInner);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Increase Build Version", new GUILayoutOption[0]);
            shouldAutoIncrement = EditorGUILayout.Toggle(shouldAutoIncrement, new GUILayoutOption[0]);
            if (shouldAutoIncrement)
            {
                m_autoIncrementValue = EditorGUILayout.TextField(m_autoIncrementValue, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Auto Run", new GUILayoutOption[0]);
            shouldAutoRun = EditorGUILayout.Toggle(shouldAutoRun, new GUILayoutOption[0]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal(styleHelpboxInner);
            if (GUILayout.Button("Apply & Save", GUILayout.MinWidth(180)))
            {
                if (File.Exists(keystorePath))
                {
                    _config.keystorePath = keystorePath;
                    PlayerSettings.Android.keystoreName = keystorePath;
                }
                bool hasLetters = Regex.Matches(m_autoIncrementValue, @"[a-zA-Z]").Count > 0;
                if (hasLetters) m_autoIncrementValue = "0";
                EditorPrefs.SetString(m_autoIncrementKey, m_autoIncrementValue);
                PlayerSettings.applicationIdentifier = currentBundleIdentifier;
                EditorPrefs.SetBool(m_autoIncreaseVersion, shouldAutoIncrement);
                EditorPrefs.SetBool(m_autoRunKey, shouldAutoRun);

                PlayerSettings.companyName = companyName;
                PlayerSettings.productName = projectName;
                _config.companyName = companyName;
                _config.projectName = projectName;
                _config.keystoreAliasName = keystoreAliasName;
                _config.keystoreAliasPassword = keystorePassword;
                _config.keystorePassword = keystorePassword;
                _config.keystorePath = keystorePath;
                SaveEditorData();
            }
            if (GUILayout.Button("Check For Update", GUILayout.MinWidth(180)))
            {
                CheckForUpdate();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(styleHelpboxInner);
            string btnBuildName = shouldAutoRun ? "Build & Run" : "Build";
            string btnBuildWithKeystoreName = shouldAutoRun ? "Build & Run with Keystore" : "Build with Keystore";
            if (GUILayout.Button(btnBuildName, GUILayout.MinWidth(180)))
            {
                BuildWithoutKeystore();
            }
            if (GUILayout.Button(btnBuildWithKeystoreName, GUILayout.MinWidth(180)))
            {
                BuildWithKeystore();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        static void CheckForUpdate()
        {
            string repoURL = "https://api.github.com/repos/prashant-singh/project-wizard/releases";
            WWW www = new WWW(repoURL);
            while (!www.isDone) ;
            if (www.isDone)
            {
                var modifiedResponse = "{\"repos\":" + www.text + "}";
                GitReponse m_gitRepo = JsonUtility.FromJson<GitReponse>(modifiedResponse);
                bool hasUpdate = false;
                string newVersion = "";
                string downloadURL = "";
                for (int count = 0; count < m_gitRepo.repos.Count; count++)
                {

                    var version1 = new System.Version(m_gitRepo.repos[count].tag_name);
                    var version2 = new System.Version(toolVersion);

                    var result = version1.CompareTo(version2);
                    if (result > 0)
                    {
                        newVersion = m_gitRepo.repos[count].tag_name;
                        hasUpdate = true;
                        break;
                    }
                }
                if (hasUpdate)
                {
                    bool selectedOp = EditorUtility.DisplayDialog("Update Available!", "Version " + newVersion + " available to download.", "Update", "Cancel");
                    if (selectedOp)
                    {
                        downloadURL = "https://github.com/googleads/googleads-mobile-unity/releases/download/v3.15.1/GoogleMobileAds.unitypackage";
                        DownloadNewPackage(downloadURL, newVersion);
                    }
                }
            }
        }

        static void DownloadNewPackage(string downloadURL, string versionNum)
        {
            string filePath = Environment.CurrentDirectory + "/ProjectSetup_" + versionNum + ".unitypackage";
            if (!File.Exists(filePath))
            {
                UnityWebRequest www = UnityWebRequest.Get(downloadURL);
                www.SendWebRequest();
                while (!www.isDone)
                {
                    EditorUtility.DisplayProgressBar("Downloading", (www.downloadProgress * 100).ToString("0") + "%", www.downloadProgress);
                }
                byte[] downloadedPackageFile = www.downloadHandler.data;
                File.WriteAllBytes(filePath, downloadedPackageFile);
                while (!File.Exists(filePath)) ;
            }
            AssetDatabase.ImportPackage(filePath, true);
        }

#if !SHOULD_AUTO_RUN
    [MenuItem("[Master_Tools]/Android/Build with keystore")]
    static void BuildWithKeystore()
    {
        BuildGame(true);
    }

    [MenuItem("[Master_Tools]/Android/Build")]
    static void BuildWithoutKeystore()
    {
        BuildGame(false);
    }
#elif SHOULD_AUTO_RUN
        [MenuItem("[Master_Tools]/Android/Build and Run with keystore")]
        static void BuildWithKeystore()
        {
            BuildGame(true);
        }

        [MenuItem("[Master_Tools]/Android/Build and Run")]
        static void BuildWithoutKeystore()
        {
            BuildGame(false);
        }
#endif

        // [MenuItem("[Master_Tools]/Build for Android %b")]
        public static void BuildGame(bool withKeystore)
        {
            RefreshKeystoreValues();
            if (!IsIdentifierSet())
            {
                Init();
            }
            else
            {
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                if (withKeystore)
                {
                    PlayerSettings.Android.keystoreName = keystorePath;
                    PlayerSettings.keystorePass = keystorePassword;
                    PlayerSettings.keyaliasPass = keystoreAliasPass;
                    PlayerSettings.Android.keyaliasName = keystoreAliasName;
                }
                else
                {
                    PlayerSettings.Android.keystoreName = "";
                    PlayerSettings.keystorePass = "";
                    PlayerSettings.keyaliasPass = "";
                    PlayerSettings.Android.keyaliasName = "";
                    var finalBundleVersion = EditorPrefs.GetBool(m_autoIncreaseVersion) ? (float.Parse(PlayerSettings.bundleVersion) + 0.01f).ToString() : PlayerSettings.bundleVersion;
                    PlayerSettings.bundleVersion = finalBundleVersion;
                }

                buildPlayerOptions.locationPathName = "BUILDS/Android/" + PlayerSettings.productName + "_" + PlayerSettings.bundleVersion + ".apk";
                buildPlayerOptions.target = BuildTarget.Android;

                buildPlayerOptions.options = shouldAutoRun ? BuildOptions.AutoRunPlayer : BuildOptions.None;
                BuildPipeline.BuildPlayer(buildPlayerOptions);
            }
        }
        static string[] GetAllSceneNames()
        {
            string[] tempAllSceneNames = new string[UnityEngine.SceneManagement.SceneManager.sceneCount];

            for (int count = 0; count < UnityEngine.SceneManagement.SceneManager.sceneCount; count++)
            {
                tempAllSceneNames[count] = UnityEngine.SceneManagement.SceneManager.GetSceneAt(count).path;
            }
            return tempAllSceneNames;
        }

        static bool IsIdentifierSet()
        {
            if (PlayerSettings.applicationIdentifier.ToLower().Equals("com.company.productname"))
            {
                return false;
            }
            else
                return true;
        }


        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // _config = new GameConfigData();
            LoadOldData();
            SaveEditorData();
            EditorUtility.RevealInFinder(pathToBuiltProject);
        }

        static void SaveEditorData()
        {
            if (!string.IsNullOrEmpty(PlayerSettings.Android.keystoreName))
            {
                _config.keystorePath = PlayerSettings.Android.keystoreName;
                _config.gameVersionWhenSignedWithKeystore = PlayerSettings.bundleVersion;
                _config.bundleVersionCodeWhenSignedWithkeystore = PlayerSettings.Android.bundleVersionCode.ToString();
                _config.lastSignedBuildDate = DateTime.Now.ToLongDateString();
            }
            else
            {
                if (_config.keystorePath.Length > 0)
                {
                    PlayerSettings.Android.keystoreName = _config.keystorePath;
                }
            }
            PlayerSettings.Android.keystorePass = _config.keystorePassword.Length > 0 ? _config.keystorePassword : PlayerSettings.Android.keystorePass;
            PlayerSettings.Android.keyaliasName = _config.keystoreAliasName.Length > 0 ? _config.keystoreAliasName : PlayerSettings.Android.keyaliasName;
            PlayerSettings.Android.keyaliasPass = _config.keystoreAliasPassword.Length > 0 ? _config.keystoreAliasPassword : PlayerSettings.Android.keyaliasPass;

            _config.GameBundleIdentifier = PlayerSettings.applicationIdentifier;
            _config.lastBuildDate = DateTime.Now.ToLongDateString();
            _config.gameVersion = Application.version;
            _config.bundleVersionCode = PlayerSettings.Android.bundleVersionCode.ToString();
            _config.unityVersion = Application.unityVersion;
            var _data = JsonUtility.ToJson(_config);
            SaveDataPhysically(_data);
        }

        static void SaveDataPhysically(string jsonData)
        {
            File.WriteAllText(Environment.CurrentDirectory + "/Assets/" + "GameConfig.json", jsonData);
        }

        private void OnDestroy()
        {
            if (shouldAutoRun)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "SHOULD_AUTO_RUN");
            }
            else
            {
                string allScriptingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, allScriptingSymbols.Replace("SHOULD_AUTO_RUN", ""));
            }
        }
    }


    [Serializable]
    public class Author
    {
        public string login;
        public int id;
        public string node_id;
        public string avatar_url;
        public string gravatar_id;
        public string url;
        public string html_url;
        public string followers_url;
        public string following_url;
        public string gists_url;
        public string starred_url;
        public string subscriptions_url;
        public string organizations_url;
        public string repos_url;
        public string events_url;
        public string received_events_url;
        public string type;
        public bool site_admin;
    }

    [Serializable]
    public class Uploader
    {
        public string login;
        public int id;
        public string node_id;
        public string avatar_url;
        public string gravatar_id;
        public string url;
        public string html_url;
        public string followers_url;
        public string following_url;
        public string gists_url;
        public string starred_url;
        public string subscriptions_url;
        public string organizations_url;
        public string repos_url;
        public string events_url;
        public string received_events_url;
        public string type;
        public bool site_admin;
    }

    [Serializable]
    public class Asset
    {
        public string url;
        public int id;
        public string node_id;
        public string name;
        public object label;
        public Uploader uploader;
        public string content_type;
        public string state;
        public int size;
        public int download_count;
        public DateTime created_at;
        public DateTime updated_at;
        public string browser_download_url;
    }

    [Serializable]
    public class Repos
    {
        public string url;
        public string assets_url;
        public string upload_url;
        public string html_url;
        public int id;
        public string node_id;
        public string tag_name;
        public string target_commitish;
        public string name;
        public bool draft;
        public Author author;
        public bool prerelease;
        public DateTime created_at;
        public DateTime published_at;
        public List<Asset> assets;
        public string tarball_url;
        public string zipball_url;
        public string body;
    }

    [Serializable]
    public class GitReponse
    {
        public List<Repos> repos;
    }
}

using System.Collections.Generic;
using Core.Audio;
using Core.Enemy;
using Core.StageGeneration.Stage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ObjectiveSystem : MonoBehaviour
    {
        // Singleton instance
        public static ObjectiveSystem instance { get; private set; }

        [SerializeField]
        private GameObject header;

        [SerializeField]
        private GameObject objectivesHolder;

        [SerializeField]
        private GameObject objectivePrefab;

        [SerializeField]
        private int enemyKillObjective;

        private int enemiesKilledCount;
        private CanvasScaler canvasScaler;
        private float screenRatio;
        private List<Objective> objectives = new List<Objective>();

        private void Awake()
        {
            if (instance is null)
                instance = this;
            else
                Destroy(gameObject);

            // Get the Canvas Scaler component
            canvasScaler = GetComponentInParent<CanvasScaler>();

            EnemyBase.OnEnemyKilled += HandleEnemyKilled;

            objectivesHolder = GameObject.Find("ObjectivesHolder");
            objectives.Add(new Objective(ObjectiveTask.KILL_Enemies, "Kill " + enemyKillObjective + " enemies (" + enemiesKilledCount + ")"));
            objectives.Add(new Objective(ObjectiveTask.COLLECT_KEY, "Find the bossroom key"));
            objectives.Add(new Objective(ObjectiveTask.KILL_BOSS, "Defeat the Boss"));

            UpdateObjectiveUI();
        }

        private void CompleteObjective(ObjectiveTask task)
        {
            Objective objective = objectives.Find(o => o.task == task);
            Transform objectiveUI = objectivesHolder.transform.Find(objective.task.ToString());
            if (objective is null || objectiveUI is null)
            {
                Debug.LogWarning("Objective not found: " + task.ToString());
                return;
            }

            objective.completed = true;
            objective.color = Color.green;

            // Get the TextMeshProUGUI component of the UI element and set its color
            TextMeshProUGUI objectiveText = objectiveUI.GetComponent<TextMeshProUGUI>();
            objectiveText.color = objective.color;

            SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.OBJECTIVE_COMPLETED);
            GameStats.Instance.objectivesCompleted++;
        }

        public bool IsObjectiveCompleted(ObjectiveTask task)
        {
            Objective objective = objectives.Find(o => o.task == task);
            return objective.completed;
        }

        private void OnDestroy()
        {
            EnemyBase.OnEnemyKilled -= HandleEnemyKilled;
            objectives.Clear();
            instance = null;
        }

        public void HandleKeycardCollected()
        {
            CompleteObjective(ObjectiveTask.COLLECT_KEY);
        }

        public void HandleEnemyKilled()
        {
            if (IsObjectiveCompleted(ObjectiveTask.KILL_Enemies))
                return;

            enemiesKilledCount++;
            if (enemiesKilledCount >= enemyKillObjective)
            {
                CompleteObjective(ObjectiveTask.KILL_Enemies);
            }

            if (enemiesKilledCount <= enemyKillObjective)
            {
                Objective objective = objectives.Find(o => o.task == ObjectiveTask.KILL_Enemies);
                Transform objectiveUI = objectivesHolder.transform.Find(objective.task.ToString());
                objectiveUI.GetComponent<TextMeshProUGUI>().text = "Kill " + enemyKillObjective + " enemies (" + enemiesKilledCount + ")";
            }
        }

        public void HandleBossKilled()
        {
            CompleteObjective(ObjectiveTask.KILL_BOSS);
        }

        private void UpdateObjectiveUI()
        {
            // Clear existing objectives
            objectivesHolder.transform.DetachChildren();

            for (int i = 0; i < objectives.Count; i++)
            {
                Objective objective = objectives[i];

                // Create a new child object
                GameObject childObject = Instantiate(objectivePrefab, objectivesHolder.transform);
                childObject.name = objective.task.ToString();

                TextMeshProUGUI textMeshProComponent = childObject.GetComponent<TextMeshProUGUI>();
                textMeshProComponent.color = objective.color;
                textMeshProComponent.text = objective.name;
                textMeshProComponent.outlineColor = Color.black;
                textMeshProComponent.fontStyle = FontStyles.Bold;
                textMeshProComponent.outlineWidth = 0.3f;

                // Calculate the scaled size of the text based on the screen size
                textMeshProComponent.fontSize = textMeshProComponent.fontSize * GetScreenRatio() + 3;
            }

            // Calculate the scaled size of the header based on the screen size
            TextMeshProUGUI headerText = header.GetComponent<TextMeshProUGUI>();
            headerText.fontSize = headerText.fontSize * GetScreenRatio();
        }

        private float GetScreenRatio()
        {
            if (canvasScaler is null || canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                return 1f;

            float referenceHeight = canvasScaler.referenceResolution.y;
            float currentHeight = Screen.height;
            return currentHeight / referenceHeight;
        }
    }

    public enum ObjectiveTask
    {
        COLLECT_KEY,
        KILL_Enemies,
        KILL_BOSS,
    }

    public class Objective
    {
        public ObjectiveTask task;
        public string name;
        public bool completed;
        public Color color;

        public Objective(ObjectiveTask index, string n)
        {
            task = index;
            name = n;
            completed = false;
            color = Color.white;
        }
    }
}
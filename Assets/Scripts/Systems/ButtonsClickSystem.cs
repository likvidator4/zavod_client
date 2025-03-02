﻿using Leopotam.Ecs;
using System;
using Components;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Systems
{
    class ButtonsClickSystem : IEcsRunSystem
    {
        private readonly EcsFilter<ButtonClickEvent> clicks = null;
        private readonly EcsWorld world = null;
        private readonly EcsFilter<BuildingComponent> builds = null;
        private readonly EcsFilter<PlayerResourcesComponent> resources = null;
        private readonly EcsFilter<UiCanvasesComponent> canvases = null;

        public void Run()
        {
            var clickedButtonsEvents = clicks.Entities.Where(x => x.IsNotNullAndAlive());

            foreach (var btn in clickedButtonsEvents)
            {
                if (btn.Get<ButtonClickEvent>() == null)
                    continue;

                btn.Unset<ButtonClickEvent>();
                HandleButtonClick(btn.Get<ButtonComponent>());
            }
        }

        private void HandleButtonClick(ButtonComponent btn)
        {
            switch (btn.buttonName)
            {
                case "CreateBarraks":
                    OnGarageCreateClick();
                    break;
                case "CreateWarrior":
                    CreateWarrior(btn);
                    break;
                case "CreateRunner":
                    CreateRunner(btn);
                    break;
                case "CreateBase":
                    CreateBase(btn);
                    break;
                case "CreateKiosk":
                    CreateKiosk();
                    break;
                case "BackToGame":
                    canvases.Get1[0].PauseMenu.enabled = false;
                    break;
                case "ExitToMainMenu":
                    ToMainMenu();
                    break;
                case "Exit":
                    world.NewEntityWith(out ExitGameEvent exit);
                    break;
                default:
                    break;
            }
        }

        private void CreateBase(ButtonComponent btn)
        {
            world.NewEntityWith(out BuildingCreateComponent buildEvent);
            buildEvent.Tag = BuildingTag.Base;
            btn.button.interactable = false;
        }

        private void ToMainMenu()
        {
            world.NewEntityWith(out ExitToMainMenuEvent exitMenuEvent);
        }

        private void CreateKiosk()
        {
            world.NewEntityWith(out BuildingCreateComponent buildEvent);
            buildEvent.Tag = BuildingTag.Stall;
        }

        private void OnGarageCreateClick()
        {
            world.NewEntityWith(out BuildingCreateComponent buildEvent);
            buildEvent.Tag = BuildingTag.Garage;
        }

        private void CreateWarrior(ButtonComponent button)
        {
            if (resources.Get1[0].Semki < 10)
                return;

            resources.Get1[0].Semki -= 10;
                
            var GameObjectOfButton = GetParentGameObjectFromButton(button.button);
            world.NewEntityWith(out UnitCreateEvent unitCreate);
            unitCreate.UnitTag = UnitTag.Warrior;
            unitCreate.PlayerId = ServerCommunication.ServerClient.Communication.userInfo.MyPlayer.Id;
            var unitPos = GameObjectOfButton == null
                ? throw new NullReferenceException()
                : GameObjectOfButton.transform.position;
            unitCreate.Position = new Vector3(unitPos.x + 5, unitPos.y, unitPos.z);
            unitCreate.Id = Guid.NewGuid();
            unitCreate.Health = 150;
        }

        private void CreateRunner(ButtonComponent button)
        {
            if (resources.Get1[0].Semki < 10)
                return;

            resources.Get1[0].Semki -= 10;

            var GameObjectOfButton = GetParentGameObjectFromButton(button.button);
            world.NewEntityWith(out UnitCreateEvent unitCreate);
            unitCreate.UnitTag = UnitTag.Runner;
            unitCreate.PlayerId = ServerCommunication.ServerClient.Communication.userInfo.MyPlayer.Id;
            var unitPos = GameObjectOfButton == null
                ? throw new NullReferenceException()
                : GameObjectOfButton.transform.position;
            unitCreate.Position = new Vector3(unitPos.x + 5, unitPos.y, unitPos.z);
            unitCreate.Id = Guid.NewGuid();
            unitCreate.Health = 150;
        }

        private GameObject GetParentGameObjectFromButton(Button button)
        {
            for (var i = 0; i < builds.GetEntitiesCount(); i++)
            {
                foreach (var inBuildButton in builds.Entities[i].Get<BuildingComponent>().AllButtons)
                {
                    if (inBuildButton.GetInstanceID() == button.GetInstanceID())
                        return builds.Get1[i].Object;
                }
            }

            return null;
        }
    }
}

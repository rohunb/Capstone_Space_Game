﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData 
{
    public GameScene currentScene;

    GameData(GameScene currentScene)
    {
        this.currentScene = currentScene;
    }
}

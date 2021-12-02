using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBookFinder : MonoBehaviour
{
    public SpellBookMngr spellBook;

    private void Start()
    {
        spellBook = GameObject.Find("SpellUnlockMngr").GetComponent<SpellBookMngr>();
    }

    public void OpenBook()
    {
        spellBook.ChangeState();
    }
}

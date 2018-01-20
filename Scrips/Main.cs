using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Main : MonoBehaviour {
	public Image press_img;
	public Text pun_text, vel_text;
	public Slider sld;
	public Boton der_bot, izq_bot, start_bot, volver_bot, comp_bot;
	public GameObject fin_obj;
	public GameObject[] flechas;
	public Animator anim;
	public float muestra, espera, min_t, ini_t;
	public int ran, puntaje;
	public bool apreta, over, presed, start, seguro;
	void Start () {
		espera = 0.25f;
		muestra = 2;
		sld.onValueChanged.AddListener(delegate {actualiza();});
	}
	IEnumerator empieza(){
		seguro = true;
		anim.SetBool("ok",true);
		yield return new WaitUntil(() => press_img.rectTransform.anchoredPosition.y > 1800);
		muestra = ini_t;
		StartCoroutine(lazo());
		press_img.gameObject.SetActive(false);
		start = true;
	}
	IEnumerator lazo(){
		ran = Random.Range(0,3);//if(ran == 2) ran = Random.Range(0,2);//borrar
		apreta = true;
		flechas[ran].SetActive(true);
		yield return new WaitForSecondsRealtime(muestra);
		if(presed){
			flechas[ran].SetActive(false);
			apreta = false;
			presed = false;
			puntaje++;
			if(muestra > min_t){
				muestra = muestra/1.02f;
			}
			//decrementa el tiempo
			yield return new WaitForSecondsRealtime(espera);
			StartCoroutine(lazo());
		}else{
			over = true;
		}
	}
	void ejecuta(bool isder){
		if(apreta){
			switch(ran){
			case 0://se deveria apretar derecha
				if(isder){
					presed = true;
				}else{
					over = true;
				}
				break;
			case 1://se deveria apretar izquierda
				if(!isder){
					presed = true;
				}else{
					over = true;
				}
				break;
			case 2://se deveria apretar los dos
				if(isder){
					if(izq_bot.pulsa){
						presed = true;
					}
				}else{
					if(der_bot.pulsa){
						presed = true;
					}
				}
				break;
			}
		}else{
			over = true;
		}
	}

	void Update () {
		if(start){
			if(der_bot.down){
				ejecuta(true);
			}
			if(izq_bot.down){
				ejecuta(false);
			}
			if(over){//se perdio la partida
				if(!fin_obj.activeSelf){
					sld.value = ini_t;//se pasa el valor de ini_t a slider
					actualiza();
					pun_text.text = "Felicidades,\n llegaste a\n"+puntaje+" toques\nVelocidad: "+System.Math.Round(muestra,2);
					fin_obj.SetActive(true);
				}else{
					if(volver_bot.up){ 
						puntaje = 0;
						over = false;
						flechas[ran].SetActive(false);
						fin_obj.SetActive(false);
						ini_t = sld.value;//se pasa el valor del slider a ini_t
						muestra = ini_t;
						StartCoroutine(lazo());
					}
					if(comp_bot.up){
						Debug.Log("comparte");
					}
				}
			}
		}else{
			if(start_bot.up && !seguro){
				StartCoroutine(empieza());
			}
		}
	}
	public void actualiza(){
		vel_text.text = " "+System.Math.Round(sld.value,2)+" ";
	}
}
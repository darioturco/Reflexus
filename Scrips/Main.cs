using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class Main : MonoBehaviour {
	public Image press_img, der_img, izq_img, fle_img, win_img, vol_img, sha_img, vel_img;
	public Text pun_text, vel_text, velf_text, sha_text, vol_text;
	public Slider sld;
	public Boton der_bot, izq_bot, start_bot, volver_bot, comp_bot, red_bot, gre_bot, blu_bot;
	public GameObject fin_obj;
	public GameObject[] flechas;
	public Color[] claro, oscuro, solido, victoria;
	public Animator anim;
	public float muestra, espera, min_t, ini_t;
	public int ran, puntaje, cont, i;
	public bool apreta, over, presed, start, seguro, isProcessing, isFocus;
	private UnityADs ads;
	void Start () {
		ads = GetComponent<UnityADs>();
		sld.onValueChanged.AddListener(delegate {actualiza();});
	}
	IEnumerator ShareScreenshot(){
		isProcessing = true;
		yield return new WaitForEndOfFrame();
		ScreenCapture.CaptureScreenshot("screenshot.png", 2);
		string destination = Path.Combine(Application.persistentDataPath, "screenshot.png");
		yield return new WaitForSecondsRealtime(0.3f);
		if(!Application.isEditor){
			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),uriObject);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),"Can you beat my score?");
			intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",intentObject, "Share your new score");
			currentActivity.Call("startActivity", chooser);
			yield return new WaitForSecondsRealtime(1);
		}
		yield return new WaitUntil(() => isFocus);
		isProcessing = false;
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
		ran = Random.Range(0,3);
		apreta = true;
		flechas[ran].SetActive(true);
		for(i=0;i<20;i++){
			yield return new WaitForSecondsRealtime(muestra/20);
			if(presed){
				i = 21;
			}
		}
		if(presed){
			flechas[ran].SetActive(false);
			apreta = false;
			presed = false;
			puntaje++;
			if(muestra > min_t){
				muestra = muestra/1.02f;
			}//decrementa el tiempo
			yield return new WaitForSecondsRealtime(espera);
			StartCoroutine(lazo());
		}else{
			over = true;
		}
	}
	IEnumerator reinicia_cor(bool ad){
		if(ad){//espera hasta que termina el ads
			yield return new WaitUntil(() => ads.startAd==false);
		}
		puntaje = 0;
		over = false;
		flechas[ran].SetActive(false);
		fin_obj.SetActive(false);
		ini_t = sld.value;//se pasa el valor del slider a ini_t
		muestra = ini_t;
		StartCoroutine(lazo());
	}
	void cambia_color(int num){
		der_img.color = new Color(solido[num].r,solido[num].g,solido[num].b,der_img.color.a);
		izq_img.color = new Color(solido[num].r,solido[num].g,solido[num].b,izq_img.color.a);
		fle_img.color = new Color(oscuro[num].r,oscuro[num].g,oscuro[num].b,fle_img.color.a);
		win_img.color = new Color(oscuro[num].r,oscuro[num].g,oscuro[num].b,win_img.color.a);
		pun_text.color = new Color(victoria[num].r,victoria[num].g,victoria[num].b,pun_text.color.a);
		velf_text.color = new Color(solido[num].r,solido[num].g,solido[num].b,velf_text.color.a);
		vel_text.color = new Color(solido[num].r,solido[num].g,solido[num].b,vel_text.color.a);
		sha_text.color = new Color(solido[num].r,solido[num].g,solido[num].b,sha_text.color.a);
		vol_text.color = new Color(solido[num].r,solido[num].g,solido[num].b,vol_text.color.a);
		vol_img.color = new Color(claro[num].r,claro[num].g,claro[num].b,vol_img.color.a);
		sha_img.color = new Color(claro[num].r,claro[num].g,claro[num].b,sha_img.color.a);
		vel_img.color = new Color(claro[num].r,claro[num].g,claro[num].b,vel_img.color.a);
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
	public void ShareBtnPress(){
		if(!isProcessing){
			StartCoroutine(ShareScreenshot());
		}
	}
	private void OnApplicationFocus (bool focus) {
		isFocus = focus;
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
						if(cont >= 6){
							ads.ShowAds();
							cont = 0;
							StartCoroutine(reinicia_cor(true));
						}else{
							cont++;
						}
						StartCoroutine(reinicia_cor(false));

					}
					if(comp_bot.up){
						ShareBtnPress();
					}
					if(red_bot.up){
						cambia_color(0);
					}
					if(gre_bot.up){
						cambia_color(1);
					}
					if(blu_bot.up){
						cambia_color(2);
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
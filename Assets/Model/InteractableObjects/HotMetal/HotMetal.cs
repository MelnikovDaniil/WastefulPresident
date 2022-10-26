using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScanResult
{
    public int GenerationNumber { get; set; }

    public float Occupancy { get; set; }
}

public class HotMetal : MonoBehaviour
{
    public Vector2 size = new Vector2(2, 2);
    public float particleRadius;
    public float occupancy = 0.80f;
    public float hesitation = 0.1f;
    public float precision = 0.15f;
    public Rigidbody2D particlePrefab;
    public GameObject coolMetalPrefab;

    private List<Rigidbody2D> particles = new List<Rigidbody2D>();
    private Dictionary<Vector2, ScanResult> scanResults = new Dictionary<Vector2, ScanResult>();

    public void Start()
    {
        GenerateHotMetal();
        StartCoroutine(SeethingRoutine());
        StartCoroutine(ScannerRoutine());
    }
    
    private IEnumerator ScannerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            ScanMap();
        }
    }

    private void ScanMap()
    {
        //scanResultss Сканирование карты и запись информации о всех ячкейках в которых был металл.
        // Если ячейка заполнена металлом +- 5% от изначального в течении 5 сканов, то происходит застывание
        var standartSParticle = Mathf.Pow(particleRadius, 2) * Mathf.PI;
        var finishPoint = new Vector2(CameraManager.Instance.minSize.x / 2, CameraManager.Instance.minSize.y / 2);
        var startPoint = -finishPoint;
        for (int x = (int)startPoint.x; x < finishPoint.x; x += 2)
        {
            for (int y = (int)startPoint.y; y < finishPoint.y; y += 2)
            {
                var scanPosition = new Vector2(x + 1, y + 1);
                var colliders = Physics2D.OverlapBoxAll(scanPosition, Vector2.one * 2, 0, LayerMask.GetMask("Fluids"));
                var sHotMetal = colliders.Count() * standartSParticle * 2 / occupancy;
                var scanOccupancy = sHotMetal / (size.x * size.y);
                if (scanResults.TryGetValue(scanPosition, out var scanResult))
                {
                    if (scanOccupancy != 0
                        && scanOccupancy >= scanResult.Occupancy - precision
                        && scanOccupancy <= scanResult.Occupancy + precision)
                    {
                        if (scanResult.GenerationNumber == 5)
                        {
                            var coolMetalBlock = Instantiate(coolMetalPrefab, scanPosition, Quaternion.identity);
                            coolMetalBlock.transform.localScale = new Vector3(1, Mathf.Clamp(scanOccupancy, 0.1f, 1f));
                            coolMetalBlock.transform.position += new Vector3(0, -(1f - scanOccupancy));
                            colliders.ToList().ForEach(x => x.gameObject.SetActive(false));
                            scanResult.GenerationNumber = 1;
                        }
                        else
                        {
                            scanResult.GenerationNumber++;
                        }
                    }
                    else
                    {
                        scanResult.GenerationNumber = 1;
                    }
                }
                else
                {
                    var newScanResult = new ScanResult
                    {
                        Occupancy = scanOccupancy,
                        GenerationNumber = 1,
                    };
                    scanResults.Add(scanPosition, newScanResult);
                }
            }
        }
    }

    private IEnumerator SeethingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            var particle = particles.GetRandom();
            particle.AddForce(Vector2.up * 20, ForceMode2D.Impulse);
        }
    }

    private void GenerateHotMetal()
    {
        var sHotMetal = size.x * size.y * occupancy;
        var sParticle = Mathf.Pow(particleRadius, 2) * Mathf.PI;
        var particelsCount = sHotMetal / sParticle;
        for (int i = 0; i < (int)particelsCount; i++)
        {
            var particleSizeCf = Random.Range(-hesitation, hesitation) + 1f;
            var position = new Vector2(
                transform.position.x + Random.Range(-size.x, size.x) / 2f,
                transform.position.y + Random.Range(-size.y, size.y) / 2f);
            var particle = Instantiate(particlePrefab, position, Quaternion.identity);
            particle.transform.localScale = Vector2.one * particleRadius * 2f * particleSizeCf;
            particles.Add(particle);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (Color.red + Color.yellow);
        Gizmos.DrawWireCube(transform.position, size);
    }

}

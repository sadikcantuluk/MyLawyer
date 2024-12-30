# Avukatım Yanımda - Anayasa Hukuk Asistanı

Avukatım Yanımda, Türkiye Anayasası hakkında sorularınızı yanıtlayan yapay zeka destekli bir hukuk asistanıdır. ChatPDF API'si kullanılarak geliştirilmiş bu uygulama, kullanıcıların anayasa ile ilgili sorularına hızlı ve doğru yanıtlar vermektedir.

## Özellikler

- 💬 Anında yanıt veren sohbet arayüzü
- 📚 Güncel Türkiye Anayasası bilgisi
- 🤖 ChatPDF API entegrasyonu
- 🎯 Kullanıcı dostu arayüz
- 📱 Responsive tasarım

## Teknolojiler

- ASP.NET Core 7.0
- C#
- JavaScript/jQuery
- HTML5/CSS3
- ChatPDF API
- Bootstrap

## Kurulum

1. Projeyi klonlayın:
```bash
git clone https://github.com/sadikcantuluk/MyLawyer.git
```

2. Proje dizinine gidin:
```bash
cd MyLawyer
```

3. Gerekli paketleri yükleyin:
```bash
dotnet restore
```

4. `appsettings.json` dosyasında ChatPDF API anahtarınızı ayarlayın. API anahtarınızı güvenli bir şekilde yönetmek için:
   - Bir `secrets.json` dosyası oluşturun (Visual Studio'da Sağ Tık > Manage User Secrets)
   - Veya ortam değişkeni olarak ayarlayın:
```json
{
  "ChatPDF": {
    "ApiKey": "your-api-key-here"
  }
}
```

5. Projeyi çalıştırın:
```bash
dotnet run
```

## Kullanım

1. Ana sayfada "Sohbete Başla" butonuna tıklayın
2. Anayasa ile ilgili sorularınızı sorun
3. Yapay zeka asistanı size yanıt verecektir

## Güvenlik

- API anahtarınızı asla doğrudan kaynak kodunda veya versiyon kontrol sisteminde saklamayın
- Development ortamı için User Secrets kullanın
- Production ortamı için güvenli ortam değişkenleri veya yapılandırma yönetim sistemleri kullanın

## Katkıda Bulunma

1. Bu repository'yi fork edin
2. Yeni bir branch oluşturun (`git checkout -b feature/yeniOzellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/yeniOzellik`)
5. Pull Request oluşturun

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına bakın.

## İletişim

E-Posta = sadikcantuluk@gmail.com
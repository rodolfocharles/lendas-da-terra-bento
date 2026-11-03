# Lendas da Terra: A Jornada de Bento

Jogo **2D Action-Adventure / Puzzle RPG side-scroller** com progressão estilo metroidvania leve, desenvolvido em **Unity 6 (URP 2D)** e **C#**.

Bento — etnógrafo, arqueólogo e folclorista — percorre biomas brasileiros investigando lendas e guardiões que foram corrompidos pelo extrativismo ilegal do **Barão Valério da Rocha**. O objetivo não é exterminar o folclore, mas **purificar as entidades, proteger a fauna e restaurar a cor e a música** de cada região.

---

## Pilares do Jogo

1. **Purificação, não extermínio:** entidades espirituais possuem **Corrupção (100% → 0%)** e rituais específicos; inimigos industriais têm vida comum.
2. **Restauração do belo:** áreas cinzentas, fumaça e águas turvas voltam à aquarela viva e à trilha regional após a purificação.
3. **Afeto e cotidiano:** moradores locais dão pistas, contam histórias, trocam itens e celebram o retorno da harmonia.
4. **Ciclo do pesquisador:** conversar com moradores → pesquisar no diário do avô e no Caderno de Campo → coletar ingredientes/reagentes → cozinhar/preparar na base móvel → realizar o ritual híbrido.

---

## Estado Atual do Protótipo (Capítulo 1 — Amazônia: *O Clamor dos Igarapés*)

O protótipo da fatia vertical Norte já é jogável do início na Barca até o ritual híbrido do **Curupira**. Ao dar **Play** no Unity, o `BootstrapPrototipo` e o `LayoutPrototipo` montam e posicionam automaticamente toda a fase (não é obrigatório rodar o menu `Lendas/` antes de testar).

### Faixas da Fase (Esquerda → Direita)

| Faixa | Conteúdo no Protótipo |
|---|---|
| **Barca de Rio** | Base móvel (`E` para entrar/sair) com **Fogão a Lenha**, **Bancada de Alquimia** e **Mesa de Pesquisa** (diário do avô). |
| **Vila** | **Dona Nair** (entrega 1º Tacacá + fitas coloridas), ingredientes (Caldo de Tucupi, Goma de Tapioca, Jambu, Camarão Seco, Pato Assado), **Lia** (entrega foto + espelho), pista espectral e gancho de chicote. |
| **Rio** | **Seu Zeca** (entrega pente de osso + **Pato Assado** para cozinhar para sua vizinha Tainá e avisa sobre a chave), **Chave da Canoa** oculta na lama (exige Visão Espectral `V`), **Caíco/Canoa**, encontros com **Boto** e **Iara**, ingredientes extras. **A travessia do rio (ida e volta para a vila) é feita exclusivamente pelo caíco/canoa.** |
| **Mata** | **Tainá** (faz escambo do **Pato no Tucupi** pelo **Colar de Sementes**), cipó, vegetação cortável com facão, **2 Jaulas da Fauna** do Barão, resina, **Favo de Mel**, Capanga caçador, Sombra, plataforma espectral e encontro com o **Caipora**. |
| **Ruínas Marajoaras** | Puzzle de alavanca com chicote, pisos falsos revelados pela Visão Espectral (`V`) e relíquia **Marca Marajoara**. |
| **Arena do Curupira** | Portão selado pela **Gota de Quartzo**, 3 focos espectrais de corrupção, Sombras invocadas e ritual híbrido do **Curupira**. |

---

## Controles (Protótipo)

| Tecla / Botão | Ação |
|---|---|
| `A` / `D` | Andar (ou remar quando estiver dentro do caíco/canoa) |
| `Espaço` | Pular / Pulo duplo / Soltar do chicote com impulso / Avançar diálogo |
| `E` | Interagir (NPCs, estações da Barca, entrar/sair da canoa nos cais, oferendas, focos de corrupção, portão) |
| `Clique Direito` ou `E` | Chicote (balançar em pontos de gancho, puxar alavancas, desequilibrar inimigos) |
| `W` / `S` | Recolher / Alongar a corda do chicote durante o balanço |
| `J` ou `Clique Esquerdo` | Facão de campo (combate corpo a corpo e cortar vegetação) |
| `K` (segurar e soltar) | Mirar em arco e disparar a **Besta Espiritual** (consome Flechas Purificadoras) |
| `V` (segurar) | **Visão Espectral** do Amuleto Ancestral (revela pistas, chave no cais, aura do Boto, plataformas e pisos falsos) |
| `I` | Abrir/fechar **Inventário** (permite comer pratos para ativar buffs como o *Treme*) |
| `Tab` ou `C` | Abrir/fechar **Caderno de Campo** (pistas, lendas, receitas e fraquezas) |

---

## Resumo das Mecânicas e Encontros do Cap. 1

- **Culinária (Fogão a Lenha na Barca):**
  - **Tacacá** (`1 Caldo de Tucupi + 1 Goma de Tapioca + 1 Jambu + 1 Camarão Seco`): concede o buff **Treme** (protege contra o canto da Iara) e também serve como parte da oferenda ao Boto.
  - **Pato no Tucupi** (`1 Pato Assado + 2 Caldo de Tucupi + 1 Jambu`): prato favorito da **Tainá** na entrada da Mata (usado no **escambo** pelo **Colar de Sementes**) e também concede o buff **Treme**.
- **Alquimia (Bancada na Barca):**
  - **Flechas Purificadoras** (`1 Resina + 1 Cipó` → 3 flechas): munição espiritual da besta.
  - **Novelo Encantado** (`2 Cipós + 1 Resina + 1 Fibra do Caipora`): item ritual necessário para os focos na arena do Curupira.
- **Travessia do Rio (Caíco / Canoa):**
  - Fale com **Seu Zeca** (que entrega o **Pente de Osso** e um **Pato Assado** para cozinhar para sua vizinha **Tainá**) e use a **Visão Espectral (`V`)** na margem do cais para revelar a **Chave da Canoa** na lama.
  - Toda travessia entre a Vila e a Mata (tanto na ida quanto ao voltar para a Vila) só pode ser feita embarcando no caíco nos cais (`E`).
- **Encontro — Boto:**
  - Na água turva, use a **Visão Espectral (`V`)** para revelar a aura espiritual do Boto e acerte **3 Flechas Purificadoras (`K`)**.
  - Em seguida, entregue a oferenda completa (**Tacacá + Foto da Lia**) com `E` para receber a **Essência das Águas** no Amuleto Ancestral.
- **Encontro — Iara:**
  - Sem sair da canoa, a Iara desce da margem e inicia o canto. Sem o buff **Treme** ativo, ela puxa Bento para o fundo em ~6s (é possível comer Tacacá/Pato com `E` durante o aviso).
  - Com o **Treme** ativo, ofereça a licença tradicional (**Pente de Osso + Espelho + Fitas Coloridas**) com `E` para receber a **Gota de Quartzo**.
- **Encontro — Caipora e Resgate da Fauna (Mata):**
  - Na entrada da Mata, entregue um **Pato no Tucupi** para **Tainá** em **escambo** (`E`) para receber o **Colar de Sementes**.
  - Quebre as **2 Jaulas da Fauna** do Barão (com Facão `J` ou Chicote), derrote o **Capanga caçador** e use a **Visão Espectral (`V`)** contra o assobio do **Caipora** na folhagem para acertar **2 Flechas Purificadoras (`K`)**.
  - Em seguida, ofereça a licença da mata (**Colar de Sementes + Favo de Mel**) com `E` para liberar a passagem para as Ruínas e receber a **Fibra do Caipora** (+ 3 flechas).
- **Chefe — Ritual do Curupira:**
  - Use a **Gota de Quartzo** para abrir o portão da arena.
  - Ative a **Visão Espectral (`V`)** para localizar os **3 focos de corrupção**, amarre o **Novelo Encantado** (`E`) e purifique as **Sombras** invocadas com a besta.
  - Com os 3 focos selados, 0 Sombras vivas e corrupção em 0%, realize o selamento final no Curupira (`E`) usando a **Gota de Quartzo + Essência das Águas**, desbloqueando o fragmento **Passo Invertido**.

---

## Estrutura do Projeto

```text
Assets/
└── _Project/
    ├── Scripts/
    │   ├── Amuleto/      # AmuletoAncestral, BentoVisaoEspectral, ObjetoEspectral
    │   ├── Base/         # BarcaRio, EstacaoFogao, EstacaoAlquimia, EstacaoPesquisa
    │   ├── Buffs/        # SistemaBuffs, EfeitoComida, CatalogoEfeitosComida
    │   ├── Caderno/      # CadernoCampo, PaginaCaderno, CatalogoPaginasCap1, HudPesquisa
    │   ├── Combat/       # BentoFacao, BentoBesta, ProjetilFlecha, InimigoSombra, InimigoSimples
    │   ├── Cooking/      # ReceitaDados, CatalogoReceitasNorte, CatalogoReceitasAlquimia, HUDs
    │   ├── Core/         # BootstrapPrototipo, LayoutPrototipo
    │   ├── Dialogue/     # GerenciadorDialogo, NpcDialogo, FalaDialogo
    │   ├── Editor/       # ConfiguradorPrototipo (menus Lendas/)
    │   ├── Inventory/    # InventarioBento, ItemDados, CatalogoItensNorte, ColetavelItem
    │   ├── Player/       # BentoMovimento, BentoChicote, BentoVida
    │   ├── UI/           # HudVida, HudInventario, HudBuffs, HudMunicao, HudEnergiaEspiritual, HudCaderno
    │   └── World/        # CanoaRio, EncontroBoto, EncontroIara, EncontroCurupira, puzzles e checkpoints
    ├── Data/             # ScriptableObjects de itens (gerados via menu Lendas/)
    ├── Prefabs/
    └── Scenes/
Docs/
├── GDD.md                # Documento de Design de Jogo consolidado
└── Backlog.md            # Backlog e progresso da fatia vertical (Capítulo 1)
```

---

## Como Abrir e Testar no Unity

1. Abra o projeto no **Unity 6** (com pacote URP 2D).
2. Abra a cena principal do protótipo e pressione **Play** — o `BootstrapPrototipo` garante automaticamente todos os sistemas, NPCs, itens e layout em faixas.
3. *(Opcional)* No menu superior do Editor, use:
   - `Lendas/Configurar Prototipo` para gravar camadas (`Chao`, `Gancho`, `Jogador`) e objetos diretamente na cena/prefab.
   - `Lendas/Criar Itens Norte (ScriptableObjects)` para gerar os assets em `Assets/_Project/Data/Itens/`.

---

## Documentação Adicional

- **[GDD Consolidado (`Docs/GDD.md`)](Docs/GDD.md):** visão completa de design, capítulos 1–5, culinária regional, fragmentos do amuleto e regras detalhadas do protótipo.
- **[Backlog (`Docs/Backlog.md`)](Docs/Backlog.md):** tarefas concluídas e próximos passos (restauração do belo, resgate da fauna, epílogos, economia de escambo/feira, salvamento JSON e migração para o novo Input System).

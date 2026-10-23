# Lendas da Terra — instruções para agentes

- Responder sempre em português.
- Projeto Unity 6 (URP 2D). Design de referência: `Docs/GDD.md`.
- Código do jogo fica em `Assets/_Project/` (Scripts, Data, Prefabs, Scenes, Art, Audio, Dialogue).
- Scripts de editor ficam em `Assets/_Project/Scripts/Editor/`; o menu `Lendas/` no Unity aplica configurações de cena/prefab.
- Nomes de classes, campos e comentários em português, sem acentos em identificadores.
- Ao mover scripts, mover junto o `.meta` para preservar o GUID.
- Não editar `ProjectSettings/*.asset` ou cenas manualmente com o editor aberto; preferir ferramentas de editor (`MenuItem`).
- Input atual: Input Manager antigo (Active Input Handling = Both). Migração para o Input System planejada para suporte a toque.
- Play monta o protótipo via `BootstrapPrototipo` + `LayoutPrototipo` (não depende do menu `Lendas/` para testar).
- Controles e regras do protótipo: `Docs/GDD.md` seção 12. Backlog: `Docs/Backlog.md`.

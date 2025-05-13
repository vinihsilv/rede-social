package com.example.demo.controller;

import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/messages")
public class MessageController {

    @PostMapping("/send")
    public String enviarMensagem() {
        return "mensagem enviada";
    }
}
